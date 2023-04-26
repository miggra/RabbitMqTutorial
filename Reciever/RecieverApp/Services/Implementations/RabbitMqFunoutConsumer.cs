using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public class RabbitMqFunoutConsumer: IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqOptions _options;
    private string? _queueName;

    public RabbitMqFunoutConsumer(
        ILogger<RabbitMqFunoutConsumer> logger,
        IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
        var factory = new ConnectionFactory { HostName = _options.HostName };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _options.FunoutExchangeName, type: ExchangeType.Fanout);

        // declare a server-named queue
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName,
            exchange: _options.FunoutExchangeName,
            routingKey: string.Empty);
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
