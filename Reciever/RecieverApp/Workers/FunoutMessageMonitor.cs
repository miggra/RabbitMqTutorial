using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RecieverApp.Workers;
public class FunoutMessageMonitor : BackgroundService
{
    private readonly ILogger<FunoutMessageMonitor> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private string _queueName;
    private string _exchangeName;

    public FunoutMessageMonitor(
        ILogger<FunoutMessageMonitor> logger)
    {
        _logger = logger;
        var factory = new ConnectionFactory { HostName = "localhost" };
		_connection = factory.CreateConnection();
		_channel = _connection.CreateModel();

        _exchangeName = "fanoutDemoExchange";
        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);

        // declare a server-named queue
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName,
            exchange: _exchangeName,
            routingKey: string.Empty);
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

        _channel.BasicConsume(queue: _queueName,
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