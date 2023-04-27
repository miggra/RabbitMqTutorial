using RecieverApp.Options;
using RecieverApp.Services;
namespace RecieverApp.BuilderInit;
public static class AddServicesExtension
{
    public static void ReadAndConfigureOptions(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<RabbitMqOptions>(config.GetSection(RabbitMqOptions.Section));
    }
    public static void AddServices(this IServiceCollection services)
    {
        // services.AddSingleton<IMessageConsumer, RabbitMqSimpleQueueConsumer>();
        // services.AddSingleton<IMessageConsumer, RabbitMqFunoutConsumer>();
        services.AddSingleton<IMessageConsumer, RabbitMqDirectConsumer>();
        services.AddSingleton<IDummyService, DummyService>();
    }
}