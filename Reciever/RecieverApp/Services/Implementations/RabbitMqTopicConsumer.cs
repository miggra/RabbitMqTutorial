using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Options;

namespace RecieverApp.Services;
public class RabbitMqTopicConsumer: RabbitMqConsumer, IMessageConsumer
{
    public RabbitMqTopicConsumer(
        IOptions<RabbitMqOptions> options): base(options)
    {
        _channel.ExchangeDeclare(exchange: _options.TopicExchangeName, type: ExchangeType.Topic);
        // declare a server-named queue
        _queueName = _channel.QueueDeclare().QueueName;

        if (_options.BindingKeys == null)
            return;

        foreach (var routingKey in _options.BindingKeys)
        {
            _channel.QueueBind(queue: _queueName,
                exchange: _options.TopicExchangeName,
                routingKey: routingKey);
        }
    }
}
