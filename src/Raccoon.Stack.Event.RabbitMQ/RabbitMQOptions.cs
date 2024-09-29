namespace Raccoon.Stack.Event.RabbitMQ;

public class RabbitMQOptions
{
    /// <summary>
    /// Gets or sets the connection string.
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 并行消费数量
    /// </summary>
    public ushort FetchCount { get; set; } = 10;

    /// <summary>
    /// Queue name
    /// </summary>
    public string Prefix { get; set; } = "Raccoon:";
}