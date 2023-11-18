using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Common.Extensions
{
    public static class ObservabilityExtensions
    {
        public static void ConfigureOpenTelemetry(this WebApplicationBuilder appBuilder)
        {
            // Note: Switch between Zipkin/OTLP/Console by setting UseTracingExporter in appsettings.json.
            var tracingExporter = appBuilder.Configuration.GetValue("UseTracingExporter", defaultValue: "console")!.ToLowerInvariant();

            // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
            var metricsExporter = appBuilder.Configuration.GetValue("UseMetricsExporter", defaultValue: "console")!.ToLowerInvariant();

            // Note: Switch between Console/OTLP by setting UseLogExporter in appsettings.json.
            var logExporter = appBuilder.Configuration.GetValue("UseLogExporter", defaultValue: "console")!.ToLowerInvariant();

            // Build a resource configuration action to set service information.
            Action<ResourceBuilder> configureResource = r => r.AddService(
                serviceName: appBuilder.Configuration.GetValue("ServiceName", defaultValue: "otel-test")!,
                serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName);

            // Configure OpenTelemetry tracing & metrics with auto-start using the
            // AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
            appBuilder.Services.AddOpenTelemetry()
                .ConfigureResource(configureResource)
                .WithTracing(builder =>
                {
                    // Tracing
                    switch (tracingExporter)
                    {
                        case "zipkin":
                            builder.AddZipkinExporter();

                            builder.ConfigureServices(services =>
                            {
                                // Use IConfiguration binding for Zipkin exporter options.
                                services.Configure<ZipkinExporterOptions>(appBuilder.Configuration.GetSection("Zipkin"));
                            });
                            break;

                        case "otlp":
                            builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                                otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                            });
                            break;

                        default:
                            builder.AddConsoleExporter();
                            break;
                    }
                })
                .WithMetrics(builder =>
                {
                    // Metrics

                    switch (metricsExporter)
                    {
                        case "prometheus":
                            // TODO:
                            break;
                        case "otlp":
                            builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                                otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                            });
                            break;
                        default:
                            builder.AddConsoleExporter();
                            break;
                    }
                });

            // Clear default logging providers used by WebApplication host.
            appBuilder.Logging.ClearProviders();

            // Configure OpenTelemetry Logging.
            appBuilder.Logging.AddOpenTelemetry(options =>
            {
                // Note: See appsettings.json Logging:OpenTelemetry section for configuration.

                var resourceBuilder = ResourceBuilder.CreateDefault();
                configureResource(resourceBuilder);
                options.SetResourceBuilder(resourceBuilder);

                switch (logExporter)
                {
                    case "otlp":
                        options.AddOtlpExporter(otlpOptions =>
                        {
                            // Use IConfiguration directly for Otlp exporter endpoint option.
                            otlpOptions.Endpoint = new Uri(appBuilder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                        });
                        break;
                    default:
                        options.AddConsoleExporter();
                        break;
                }
            });
        }
    }
}
