namespace RecieverApp.Services;
public class DummyService: IDummyService
{
    public async Task SimulateWorkForDotsInMessage(string message)
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
}