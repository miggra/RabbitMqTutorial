using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public class RabbitMqDirectConsumer: RabbitMqConsumer, IMessageConsumer
{
    public RabbitMqDirectConsumer(
        IOptions<RabbitMqOptions> options): base(options)
    {
        _channel.ExchangeDeclare(exchange: _options.DirectExchangeName, type: ExchangeType.Direct);

        // declare a server-named queue
        _queueName = _channel.QueueDeclare().QueueName;

        if (_options.DirectRoutingKeys == null)
            return;

        foreach (var routingKey in _options.DirectRoutingKeys)
        {
            _channel.QueueBind(queue: _queueName,
                exchange: _options.DirectExchangeName,
                routingKey: routingKey);
        }
    }
}
