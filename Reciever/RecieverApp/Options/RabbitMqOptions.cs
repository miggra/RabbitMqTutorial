namespace RecieverApp.Options
{
    public class RabbitMqOptions
    {
        public static string Section = "RabbitMQ";
        public string? HostName {get; set;}
        public string? SimpleQueueName { get; set; }
        public string? FunoutExchangeName { get; set; }
    }
}