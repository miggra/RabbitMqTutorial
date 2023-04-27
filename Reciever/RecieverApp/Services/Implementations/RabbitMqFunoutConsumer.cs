using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public class RabbitMqFunoutConsumer: RabbitMqConsumer, IMessageConsumer
{
    public RabbitMqFunoutConsumer(
        IOptions<RabbitMqOptions> options): base(options)
    {
        _channel.ExchangeDeclare(exchange: _options.FunoutExchangeName, type: ExchangeType.Fanout);
        // declare a server-named queue
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName,
            exchange: _options.FunoutExchangeName,
            routingKey: string.Empty);
    }
}
