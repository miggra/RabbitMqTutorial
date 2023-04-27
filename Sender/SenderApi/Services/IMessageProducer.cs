namespace SenderApi.Services;
public interface IMessageProducer: IDisposable
{
    void PublishToQueue(string message, string queue);
    void PublishFunout(string message, string exchangeName);
    void PublishDirect(string message, string exchange, string routingKey);
    void PublishTopic(string message, string exchange, string routingKey);
}