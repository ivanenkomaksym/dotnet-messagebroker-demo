using Common.Configuration;
using Microsoft.Extensions.Configuration;

namespace Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetConnectionString(this IConfiguration configuration)
        {
            var databaseSettings = new DatabaseSettings();
            configuration.GetSection(DatabaseSettings.Name).Bind(databaseSettings);
            var connectionString = databaseSettings.ConnectionString ?? string.Empty;
            return connectionString;
        }

        public static string GetGatewayAddress(this IConfiguration configuration)
        {
            var apiSettings = new ApiSettings();
            configuration.GetSection(ApiSettings.Name).Bind(apiSettings);
            var gatewayAddress = apiSettings.GatewayAddress ?? string.Empty;
            return gatewayAddress;
        }
    }
}