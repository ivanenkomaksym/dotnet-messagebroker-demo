using Common.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace Common.FeatureManagement
{
    public static class Helpers
    {
        public static void AddIfFeatureEnabledHttpClientBased<TServiceInterface, TServiceImpl, TStubServiceImpl>(this IServiceCollection services, string feature, string gatewayAddress)
            where TServiceInterface : class
            where TStubServiceImpl : class, TServiceInterface, new()
            where TServiceImpl : class, TServiceInterface
        {
            services.AddSingleton<TServiceInterface>(provider =>
            {
                var featureManager = provider.GetRequiredService<IFeatureManager>();

                if (featureManager.IsEnabledAsync(feature).GetAwaiter().GetResult())
                {
                    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                    var environmentRouter = provider.GetRequiredService<IEnvironmentRouter>();
                    var serviceInterfaceName = typeof(TServiceInterface).FullName;
                    ArgumentNullException.ThrowIfNull(serviceInterfaceName);
                    var httpClient = httpClientFactory.CreateClient(serviceInterfaceName);
                    httpClient.BaseAddress = new Uri(gatewayAddress);
                    var instance = Activator.CreateInstance(typeof(TServiceImpl), httpClient, environmentRouter);
                    ArgumentNullException.ThrowIfNull(instance);
                    return (TServiceImpl)instance;
                }

                return new TStubServiceImpl();
            });
        }

        public static void AddIfFeatureEnabledServiceBased<TServiceInterface, TServiceImpl, TStubServiceImpl, TDependentService>(this IServiceCollection services, string feature)
            where TServiceInterface : class
            where TStubServiceImpl : class, TServiceInterface, new()
            where TServiceImpl : class, TServiceInterface
            where TDependentService : class
        {
            services.AddSingleton<TServiceInterface>(provider =>
            {
                var featureManager = provider.GetRequiredService<IFeatureManager>();

                if (featureManager.IsEnabledAsync(feature).GetAwaiter().GetResult())
                {
                    var dependentService = provider.GetRequiredService<TDependentService>();
                    var instance = Activator.CreateInstance(typeof(TServiceImpl), dependentService);
                    ArgumentNullException.ThrowIfNull(instance);
                    return (TServiceImpl)instance;
                }

                return new TStubServiceImpl();
            });
        }
    }
}
