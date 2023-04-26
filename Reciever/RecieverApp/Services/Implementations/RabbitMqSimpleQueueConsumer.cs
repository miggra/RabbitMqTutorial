using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public class RabbitMqSimpleQueueConsumer: IMessageConsumer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
   private readonly RabbitMqOptions _options;

    public RabbitMqSimpleQueueConsumer(
        ILogger<RabbitMqSimpleQueueConsumer> logger,
        IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
        var factory = new ConnectionFactory { HostName = _options.HostName };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
		_channel.QueueDeclare(
            queue: _options.SimpleQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // prefetchCount - не выдает больше одного сообщения на обратку одновременно
        // происходит ожидание Ack
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
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

        _channel.BasicConsume(queue: _options.SimpleQueueName,
                     autoAck: false,
                     consumer: consumer);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
