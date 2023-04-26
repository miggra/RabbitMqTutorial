using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RecieverApp.Workers;
public class RabbitMessageReciever : BackgroundService
{
    private readonly ILogger<RabbitMessageReciever> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMessageReciever(
        ILogger<RabbitMessageReciever> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory { HostName = "localhost" };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();
		_channel.QueueDeclare(
            queue: "SimpleMessageQueue",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Recieved {message}");
        };

        _channel.BasicConsume(queue: "SimpleMessageQueue",
                     autoAck: true,
                     consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}