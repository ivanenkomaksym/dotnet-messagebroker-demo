using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Grpc.Health.V1;
using Common.HealthChecks;

namespace Common.Extensions
{
    public static class GrpcExtensions
    {
        /// <summary>
        /// Adds a gRPC service reference. Configures a binding between the <typeparamref name="TClient"/> type and a named <see cref="HttpClient"/>
        /// with an address and gRPC-based health check. The client name will be set to the type name of <typeparamref name="TClient"/>.
        /// </summary>
        /// <remarks>
        /// Note that the gRPC service must be configured to use gRPC health checks. See https://learn.microsoft.com/aspnet/core/grpc/health-checks for more details.
        /// </remarks>
        /// <typeparam name="TClient">The type of the gRPC client. The type specified will be registered in the service collection as a transient service.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <param name="address">
        /// The value to assign to the <see cref="Grpc.Net.ClientFactory.GrpcClientFactoryOptions.Address"/> property of the typed gRPC client's injected
        /// <see cref="Grpc.Net.ClientFactory.GrpcClientFactoryOptions"/> instance.
        /// </param>
        /// <param name="failureStatus">The <see cref="HealthStatus"/> that should be reported if the health check fails. Defaults to <see cref="HealthStatus.Unhealthy"/>.</param>
        /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to configure the client.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="address"/> is not a valid URI value.</exception>
        public static IHttpClientBuilder AddGrpcServiceReference<TClient>(this IServiceCollection services, string address, string? healthCheckName = null, HealthStatus failureStatus = default)
            where TClient : class
        {
            ArgumentNullException.ThrowIfNull(services);

            if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
            {
                throw new ArgumentException("Address must be a valid absolute URI.", nameof(address));
            }

            var uri = new Uri(address);
            var builder = services.AddGrpcClient<TClient>(o => o.Address = uri);

            AddGrpcHealthChecks(services, uri, healthCheckName ?? $"{typeof(TClient).Name}-health", failureStatus);

            return builder;
        }

        private static void AddGrpcHealthChecks(IServiceCollection services, Uri uri, string healthCheckName, HealthStatus failureStatus = default)
        {
            services.AddGrpcClient<Health.HealthClient>(o => o.Address = uri);
            services.AddHealthChecks()
                .AddCheck<GrpcServiceHealthCheck>(healthCheckName, failureStatus);
        }
    }
}
