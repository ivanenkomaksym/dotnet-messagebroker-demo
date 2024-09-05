using MassTransit.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        public static void ConfigureOpenTelemetry(this IHostBuilder hostBuilder)
        {
            hostBuilder
                .ConfigureServices((hostContext, services) =>
            {
                services.ConfigureOpenTelemetry(hostContext.Configuration);
            })
                .ConfigureLogging((hostContext, configureLogging) =>
            {
                configureLogging.ConfigureLogging(hostContext.Configuration);
            });
        }

        private static void ConfigureOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            // Note: Switch between Zipkin/OTLP/Console by setting UseTracingExporter in appsettings.json.
            var tracingExporter = configuration.GetValue("UseTracingExporter", defaultValue: "console")!.ToLowerInvariant();

            // Note: Switch between Prometheus/OTLP/Console by setting UseMetricsExporter in appsettings.json.
            var metricsExporter = configuration.GetValue("UseMetricsExporter", defaultValue: "console")!.ToLowerInvariant();

            // Build a resource configuration action to set service information.
            var configureResource = GetResourceBuilder(configuration);

            // Create a service to expose ActivitySource, and Metric Instruments
            // for manual instrumentation
            services.AddSingleton<Instrumentation>();

            // Configure OpenTelemetry tracing & metrics with auto-start using the
            // AddOpenTelemetry extension from OpenTelemetry.Extensions.Hosting.
            services.AddOpenTelemetry()
                .ConfigureResource(configureResource)
                .WithTracing(builder =>
                {
                    builder.AddSource(Instrumentation.ActivitySourceName)
                        .SetSampler(new AlwaysOnSampler())
                        .AddHttpClientInstrumentation()
                        // MassTransit source. Enables trace propagation between producer and consumer
                        .AddSource(DiagnosticHeaders.DefaultListenerName)
                        .AddAspNetCoreInstrumentation();
                    // Tracing
                    switch (tracingExporter)
                    {
                        case "zipkin":
                            builder.AddZipkinExporter();

                            builder.ConfigureServices(services =>
                            {
                                // Use IConfiguration binding for Zipkin exporter options.
                                services.Configure<ZipkinExporterOptions>(configuration.GetSection("Zipkin"));
                            });
                            break;

                        case "otlp":
                            builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
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
                    builder.AddMeter(Instrumentation.MeterName)
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();

                    switch (metricsExporter)
                    {
                        case "prometheus":
                            // TODO:
                            break;
                        case "otlp":
                            builder.AddOtlpExporter(otlpOptions =>
                            {
                                // Use IConfiguration directly for Otlp exporter endpoint option.
                                otlpOptions.Endpoint = new Uri(configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                                otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                            });
                            break;
                        default:
                            builder.AddConsoleExporter();
                            break;
                    }
                });
        }

        public static void ConfigureLogging(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            // Note: Switch between Console/OTLP by setting UseLogExporter in appsettings.json.
            var logExporter = configuration.GetValue("UseLogExporter", defaultValue: "console")!.ToLowerInvariant();

            loggingBuilder.AddOpenTelemetry(options =>
            {
                // Note: See appsettings.json Logging:OpenTelemetry section for configuration.
                var configureResource = GetResourceBuilder(configuration);
                var resourceBuilder = ResourceBuilder.CreateDefault();
                configureResource(resourceBuilder);
                options.SetResourceBuilder(resourceBuilder);

                switch (logExporter)
                {
                    case "otlp":
                        options.AddOtlpExporter(otlpOptions =>
                        {
                            // Use IConfiguration directly for Otlp exporter endpoint option.
                            otlpOptions.Endpoint = new Uri(configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317")!);
                            otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                        });
                        break;
                    default:
                        options.AddConsoleExporter();
                        break;
                }
            });
        }

        private static Action<ResourceBuilder> GetResourceBuilder(IConfiguration configuration)
        {
            // Build a resource configuration action to set service information.
            return r => r.AddService(
                serviceName: configuration.GetValue("ServiceName", defaultValue: "otel-test")!,
                serviceVersion: typeof(ObservabilityExtensions).Assembly.GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName);
        }
    }
}
