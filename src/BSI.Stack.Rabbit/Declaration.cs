using RabbitMQ.Client;

namespace BSI.Stack.Rabbit;

public interface IDeclaration
{
    void QueueDeclare(string queue, bool durable, bool autoDelete = false, bool exclusive = false,
        IDictionary<string, object> arguments = null);

    void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete = false,
        IDictionary<string, object> arguments = null);

    void QueueBind(string queue, string exchange, string routingKey,
        IDictionary<string, object> arguments = null);
}

public class DefaultDeclaration : IDeclaration
{
    private readonly IModel _chnl;

    // ReSharper disable once ConvertToPrimaryConstructor
    public DefaultDeclaration(IModel chnl)
    {
        _chnl = chnl;
    }

    public void QueueDeclare(string queue, bool durable, bool autoDelete = false, bool exclusive = false,
        IDictionary<string, object> arguments = null)
    {
        _chnl.QueueDeclare(
            queue: queue,
            durable: durable,
            autoDelete: autoDelete,
            exclusive: exclusive,
            arguments: arguments);
    }

    public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete = false,
        IDictionary<string, object> arguments = null)
    {
        _chnl.ExchangeDeclare(
            exchange: exchange,
            type: type,
            durable: durable,
            autoDelete: autoDelete,
            arguments: arguments);
    }

    public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
    {
        _chnl.QueueBind(
            queue: queue,
            exchange: exchange,
            routingKey: routingKey,
            arguments: arguments);
    }
}