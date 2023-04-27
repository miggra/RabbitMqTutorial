using SenderApi.Services;
namespace SenderApi.BuilderInit;
public static class AddServicesExtension
{
    public static void AddServices(this IServiceCollection services)
    {
        // services.AddSingleton<IMessageConsumer, RabbitMqSimpleQueueConsumer>();
        services.AddScoped<IMessageProducer, RabbitMqProducer>();
    }
}