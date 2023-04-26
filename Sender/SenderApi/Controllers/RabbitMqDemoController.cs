using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace SenderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RabbitMqDemoController : ControllerBase
{
    private readonly ILogger<RabbitMqDemoController> _logger;

    public RabbitMqDemoController(ILogger<RabbitMqDemoController> logger)
    {
        _logger = logger;
    }

    [HttpPost("send-message-to-queue", Name = "SendMessageToQueue")]
    public IActionResult SendMessageToQueue(string message)
    {
        var factory = new ConnectionFactory{ HostName = "localhost"};

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "DurableMessageQueue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(message);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true; //сообщения все равно исчезают


        channel.BasicPublish(
          exchange: String.Empty,
          routingKey: "DurableMessageQueue", // should match with queue name
          basicProperties: properties,
          body: body
        );

        return Ok();
    }

    /// <summary>
    /// Example for monitoring
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [HttpPost("send-fanout", Name = "SendFunoutMessages")]
    public IActionResult SendFunoutMessages(string message)
    {
        var factory = new ConnectionFactory{ HostName = "localhost"};

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        string exchangeName = "fanoutDemoExchange";
        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
          exchange: exchangeName,
          routingKey: String.Empty, // should match with queue name
          basicProperties: null,
          body: body
        );

        return Ok();
    }
}
