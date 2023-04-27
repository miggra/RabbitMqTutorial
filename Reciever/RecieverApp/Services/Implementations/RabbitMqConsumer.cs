using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public abstract class RabbitMqConsumer: IMessageConsumer
{
    protected readonly IConnection _connection;
    protected readonly IModel _channel;
    protected readonly RabbitMqOptions _options;
    protected string? _queueName;

    public RabbitMqConsumer(
        IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
        var factory = new ConnectionFactory { HostName = _options.HostName };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

        // initialization continues in inherited classes
    }

    public void Consume(Func<string, Task> onMessageReceivedAction)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            string message = MessageDecoder.DecodeMessage(body);
            await onMessageReceivedAction(message);
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _queueName,
                     autoAck: false,
                     consumer: consumer);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
