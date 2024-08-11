using System.Collections.Concurrent;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Raccoon.Stack.Rabbit;

public abstract class RabbitClient
{
    private readonly ILogger _logger;

    protected readonly RabbitOptions RabbitOptions;
    protected ConcurrentDictionary<IConnection, int> Connections = new();
    protected ConcurrentDictionary<string, Lazy<IModel>> Channels = new();

    protected readonly ConcurrentDictionary<string,
            Channel<(object model, BasicDeliverEventArgs args, ConsumeOptions options)>>
        Queue = new();

    public RabbitClient(ILogger<RabbitClient> logger, RabbitOptions rabbitOptions)
    {
        _logger = logger;
        RabbitOptions = rabbitOptions;

        var cf = new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(30),
            // see：https://github.com/rabbitmq/rabbitmq-dotnet-client/issues/1112
            DispatchConsumersAsync = true,
            Uri = new Uri(RabbitOptions.ConnectionString)
        };

        if (RabbitOptions.PoolSize < 1)
        {
            RabbitOptions.PoolSize = 1;
        }

        if (RabbitOptions.PoolSize > 100)
        {
            RabbitOptions.PoolSize = 100;
        }

        for (var i = 0; i < RabbitOptions.PoolSize; i++)
        {
            Connections.TryAdd(cf.CreateConnection(), 0);
        }
    }

    public void Start()
    {
        if ((RabbitOptions.Consumes?.Count ?? 0) > 0)
        {
            foreach (var opt in RabbitOptions.Consumes)
            {
                Subscribe(opt);
            }
        }
    }

    public void Subscribe(ConsumeOptions consumeOptions)
    {
        // 通过GetOrAdd确保同一个队列只有一个channel
        _ = Channels.GetOrAdd(consumeOptions.Queue,
            queue => new Lazy<IModel>(() => CreateAndConfigureChannel(queue, consumeOptions))).Value;
    }

    private IModel CreateAndConfigureChannel(string queue, ConsumeOptions consumeOptions)
    {
        var opt = consumeOptions;
        var channel = GetConnection().CreateModel();
        var decl = new DefaultDeclaration(channel);
        opt.Declaration.Invoke(decl);

        if (opt.FetchCount > 0)
        {
            channel.BasicQos(0, opt.FetchCount, false);
        }

        var consumer = new AsyncEventingBasicConsumer(channel);

        if (opt.FetchCount > 1)
        {
            SubscribeMultiple(opt, channel, consumer);
        }
        else
        {
            consumer.Received += async (obj, arg) =>
            {
                try
                {
                    await OnReceived(obj, arg, opt);
                    channel.BasicAck(arg.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"rabbit on queue({opt.Queue}) received error: {ex.ToString()}");
                    channel.BasicNack(arg.DeliveryTag, false, opt.FailedRequeue);
                }
            };
        }


        // 当通道调用的回调中发生异常时发出信号
        channel.CallbackException += (_, args) => { _logger.LogError(args.Exception, args.Exception.Message); };
        channel.BasicConsume(
            queue: opt.Queue,
            autoAck: opt.AutoAck,
            consumer: consumer);

        return channel;
    }

    /// <summary>
    /// 定义处理FetchCount大于1的情况，通过Channel实现并行处理
    /// </summary>
    private void SubscribeMultiple(ConsumeOptions opt, IModel channel, AsyncEventingBasicConsumer consumer)
    {
        var semaphore = new SemaphoreSlim(opt.FetchCount, opt.FetchCount);

        consumer.Received += async (obj, arg) =>
        {
            try
            {
                await semaphore.WaitAsync();

                var (model, args, options) = (obj, arg, opt);

                channel.BasicAck(arg.DeliveryTag, false);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await OnReceived(model, args, options);

                        channel.BasicAck(args.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"rabbit on queue({options.Queue}) received error: {ex.ToString()}");
                        channel.BasicNack(args.DeliveryTag, false, options.FailedRequeue);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"rabbit on queue({opt.Queue}) received error: {ex.ToString()}");
                channel.BasicNack(arg.DeliveryTag, false, opt.FailedRequeue);
            }
            finally
            {
                semaphore.Release();
            }
        };
    }


    public void UnSubscribe(string queue, string exchange, string routingKey, bool deleteQueue = false)
    {
        if (!Channels.TryGetValue(queue, out var channel))
        {
            return;
        }

        _logger.LogInformation($"尝试解除rabbit绑定 queue:{queue}, exchange:{exchange}, routingKey:{routingKey}");
        channel.Value.QueueUnbind(queue, exchange, routingKey);

        if (deleteQueue)
        {
            var passive = channel.Value.QueueDeclarePassive(queue);
            _logger.LogInformation(
                $"尝试删除rabbit queue:{passive.QueueName} consumerCount:{passive.ConsumerCount} messageCount:{passive.MessageCount}");
            channel.Value.QueueDelete(queue);
        }

        channel.Value.Close();
        Channels.TryRemove(queue, out _);
        Queue.TryRemove(queue, out var item);

        // 取消item中的所有等待
        item?.Writer.TryComplete();
    }

    public void Stop()
    {
        if (Channels != null)
        {
            foreach (var kv in Channels)
            {
                kv.Value.Value.Close();
            }

            Channels = null;
        }

        if (Connections != null)
        {
            foreach (var kv in Connections)
            {
                kv.Key.Close();
            }

            Connections = null;
        }
    }

    public void Publish(string exchange, string routingKey, byte[] data, Action<IBasicProperties> options = null)
    {
        using var activity = RabbitInstrumentation.ActivitySource.StartActivity($"/{exchange}/{routingKey}");
        activity?.SetTag("kind", "publish");

        var key = $"{exchange}:{routingKey}";
        var chnl = Channels.GetOrAdd(key, k => new Lazy<IModel>(GetConnection().CreateModel())).Value;
        var prop = chnl.CreateBasicProperties();
        options?.Invoke(prop);

        chnl.BasicPublish(exchange: exchange, routingKey: routingKey, body: data, basicProperties: prop);
    }

    protected abstract Task OnReceived(object model, BasicDeliverEventArgs args, ConsumeOptions options);

    private IConnection GetConnection()
    {
        var con = Connections.OrderBy(q => q.Value).Select(q => q.Key).FirstOrDefault();
        Connections.AddOrUpdate(con, k => 1, (k, v) => v + 1);
        return con;
    }
}