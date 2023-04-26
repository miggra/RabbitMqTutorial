namespace RecieverApp.Services;
public interface IMessageConsumer: IDisposable
{
    void Consume(Func<string, Task> onMessageReceivedAction);
}