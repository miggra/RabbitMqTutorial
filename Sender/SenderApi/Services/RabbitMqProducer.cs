using System.Text;
using RabbitMQ.Client;

namespace SenderApi.Services;
public class RabbitMqProducer: IMessageProducer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private string? _queueName;

    public RabbitMqProducer()
    {
        var factory = new ConnectionFactory{ HostName = "localhost"};
        _connection = factory.CreateConnection();;
        _channel = _connection.CreateModel();
    }

    public void PublishToQueue(string message, string queue)
    {
        _channel.QueueDeclare(
            queue: queue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true; //сообщения все равно исчезают


        _channel.BasicPublish(
          exchange: String.Empty,
          routingKey: queue, // should match with queue name
          basicProperties: properties,
          body: body
        );
    }

    public void PublishFunout(string message, string exchangeName)
    {
        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
          exchange: exchangeName,
          routingKey: String.Empty, // should match with queue name
          basicProperties: null,
          body: body
        );
    }

    public void PublishDirect(string message, string exchange, string routingKey)
    {
        _channel.ExchangeDeclare(exchange, ExchangeType.Direct);

        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
          exchange: exchange,
          routingKey: routingKey,
          basicProperties: null,
          body: body
        );
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}