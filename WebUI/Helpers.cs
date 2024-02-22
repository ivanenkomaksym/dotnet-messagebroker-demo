using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace WebUI
{
    public static class Helpers
    {
        public static string GetLocalUrl(IUrlHelper urlHelper, string localUrl)
        {
            if (!urlHelper.IsLocalUrl(localUrl))
            {
                return urlHelper!.Page("/Index");
            }

            return localUrl;
        }

        public static void AddIfFeatureEnabled<TServiceInterface, TServiceImpl, TStubServiceImpl>(this IServiceCollection services, string feature, string gatewayAddress)
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
                    var httpClient = httpClientFactory.CreateClient(typeof(TServiceInterface).FullName);
                    httpClient.BaseAddress = new Uri(gatewayAddress);
                    return (TServiceImpl)Activator.CreateInstance(typeof(TServiceImpl), httpClient);
                }

                return new TStubServiceImpl();
            });
        }
    }
}
