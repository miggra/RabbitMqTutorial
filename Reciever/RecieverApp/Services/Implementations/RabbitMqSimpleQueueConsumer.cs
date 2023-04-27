using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public class RabbitMqSimpleQueueConsumer: RabbitMqConsumer, IMessageConsumer
{
    public RabbitMqSimpleQueueConsumer(
        IOptions<RabbitMqOptions> options): base(options)
    {
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
}
