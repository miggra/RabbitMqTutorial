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

    [HttpGet(Name = "SendMessageToQueue")]
    public IActionResult SendMessageToQueue(string message)
    {
        var factory = new ConnectionFactory{ HostName = "localhost"};

        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(
            queue: "SimpleMessageQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
          exchange: String.Empty,
          routingKey: "SimpleMessageQueue", // should match with queue name
          basicProperties: null,
          body: body
        );

        return Ok();
    }
}
