using Common.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Common.Extensions
{
    public static class AspireExtensions
    {
        public static void ConfigureRabbitMq(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
        {
            var applicationOptions = context.GetRequiredService<IOptions<ApplicationOptions>>();
            if (applicationOptions != null && applicationOptions.Value.StartupEnvironment == StartupEnvironment.Aspire)
            {
                var configService = context.GetRequiredService<IConfiguration>();
                var connectionString = configService.GetConnectionString("messaging"); // <--- same name as in the orchestration
                cfg.Host(connectionString);
            }
        }
    }
}
