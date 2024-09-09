using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.HealthChecks
{
    public sealed class GrpcServiceHealthCheck(Health.HealthClient healthClient) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var response = await healthClient.CheckAsync(new(), cancellationToken: cancellationToken);

            return response.Status switch
            {
                HealthCheckResponse.Types.ServingStatus.Serving => HealthCheckResult.Healthy(),
                _ => HealthCheckResult.Unhealthy()
            };
        }
    }
}
