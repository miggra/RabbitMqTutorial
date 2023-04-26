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
            queue: "DurableMessageQueue",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        // prefetchCount - не выдает больше одного сообщения на обратку одновременно
        // происходит ожидание Ack
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"Recieved {message}");

            await SimulateWorkOnMessage(message);

            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: "DurableMessageQueue",
                     autoAck: false,
                     consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task SimulateWorkOnMessage(string message)
    {
        Console.WriteLine($"Start doing work");
        int dots = message.Split('.').Length - 1;
        for (int i = dots; i > 0; i--)
        {
            Console.WriteLine($"{i} seconds left");
            await Task.Delay(1000);
        }
        Console.WriteLine($"Work completed");
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}