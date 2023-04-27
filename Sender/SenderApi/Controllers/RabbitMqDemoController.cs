using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using SenderApi.Services;

namespace SenderApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RabbitMqDemoController : ControllerBase
{
    private readonly ILogger<RabbitMqDemoController> _logger;
    private readonly IMessageProducer _messageProducer;

    public RabbitMqDemoController(ILogger<RabbitMqDemoController> logger, IMessageProducer messageProducer)
    {
        _logger = logger;
        _messageProducer = messageProducer;
    }

    [HttpPost("send-message-to-queue", Name = "SendMessageToQueue")]
    public IActionResult SendMessageToQueue(string message)
    {
        string queue = "DurableMessageQueue";
        _messageProducer.PublishToQueue(message, queue);
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
        string exchangeName = "fanoutDemoExchange";
        _messageProducer.PublishFunout(message, exchangeName);
        return Ok();
    }

    /// <summary>
    /// Grouping for queue
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [HttpPost("send-direct", Name = "SendDirectMessages")]
    public IActionResult SendDirectMessages(string message, string routingKey)
    {
        string exchangeName = "directExchange";
        _messageProducer.PublishDirect(message, exchangeName, routingKey);
        return Ok();
    }
}
