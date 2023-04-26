using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RecieverApp.Services;

namespace RecieverApp.Workers;
public class MessageConsumeWorker : BackgroundService
{
    private readonly ILogger<MessageConsumeWorker> _logger;
    private readonly IMessageConsumer _messageSimpleQueueService;
    private readonly IDummyService _dummyService;

    public MessageConsumeWorker(
        ILogger<MessageConsumeWorker> logger,
        IMessageConsumer messageSimpleQueueService,
        IDummyService dummyService)
    {
        _logger = logger;
        _messageSimpleQueueService = messageSimpleQueueService;
        _dummyService = dummyService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _messageSimpleQueueService.Consume(async message =>
            await _dummyService.SimulateWorkForDotsInMessage(message));

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _messageSimpleQueueService.Dispose();
        base.Dispose();
    }
}