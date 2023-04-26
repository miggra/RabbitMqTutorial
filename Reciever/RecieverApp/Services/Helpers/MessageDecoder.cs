using System.Text;

namespace RecieverApp.Services;
public static class MessageDecoder
{
    public static string DecodeMessage(byte[] bytes)
    {
        var message = Encoding.UTF8.GetString(bytes);
        Console.WriteLine($"Recieved {message}");
        return message;
    }
}