namespace RecieverApp.Services;
public interface IDummyService
{
    Task SimulateWorkForDotsInMessage(string message);
}