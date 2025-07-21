using Common.Configuration;
using Common.Extensions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

// Adds common .NET Aspire services: service discovery, resilience, health checks, and OpenTelemetry.
// This project should be referenced by each service project in your solution.
// To learn more about using this project, see https://aka.ms/dotnet/aspire/service-defaults
public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this WebApplicationBuilder builder)
    {
        var applicationOptionsSection = builder.Configuration.GetSection(ApplicationOptions.Name);
        builder.Services.Configure<ApplicationOptions>(applicationOptionsSection);

        builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.Name));

        var applicationOptions = applicationOptionsSection.Get<ApplicationOptions>();
        if (applicationOptions == null || applicationOptions.StartupEnvironment == StartupEnvironment.Kubernetes)
        {
            builder.Host
                .ConfigureServices((hostContext, services) =>
                {
                    builder.Host.ConfigureOpenTelemetry(hostContext, services);
                });
        }
        else
        {
            builder.ConfigureAspireOpenTelemetry();
        }

        builder.AddDefaultHealthChecks();

        builder.Services.AddServiceDiscovery();

        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler(options =>
            {
                options.Retry = new HttpRetryStrategyOptions
                {
                    ShouldHandle = args =>
                    {
                        // Only retry if it's NOT a 501, AND it's a server error (5xx) or a transient network failure.
                        return ValueTask.FromResult(args.Outcome.Result?.StatusCode != System.Net.HttpStatusCode.NotImplemented);
                    },
                    MaxRetryAttempts = 1,
                    Delay = TimeSpan.FromSeconds(0), // Initial delay
                };
            });

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        return builder;
    }

    public static void AddLogging(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
    {
        var applicationOptions = configuration.GetSection(ApplicationOptions.Name).Get<ApplicationOptions>();
        if (applicationOptions == null || applicationOptions.StartupEnvironment == StartupEnvironment.Kubernetes)
        {
            loggingBuilder.ConfigureLogging(configuration);
        }
        else
        {
            loggingBuilder.ConfigureAspireLogging();
        }
    }

    public static void AddServiceDefaults(this IHostBuilder builder, HostBuilderContext hostContext, IServiceCollection services)
    {
        var applicationOptionsSection = hostContext.Configuration.GetSection(ApplicationOptions.Name);
        services.Configure<ApplicationOptions>(applicationOptionsSection);

        services.Configure<DatabaseSettings>(hostContext.Configuration.GetSection(DatabaseSettings.Name));

        var applicationOptions = applicationOptionsSection.Get<ApplicationOptions>();
        if (applicationOptions == null || applicationOptions.StartupEnvironment == StartupEnvironment.Kubernetes)
        {
            builder.ConfigureOpenTelemetry(hostContext, services);
        }
        else
        {
            builder.ConfigureAspireOpenTelemetry(hostContext, services);
        }

        services.AddDefaultHealthChecks();

        services.AddServiceDiscovery();

        services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on resilience by default
            http.AddStandardResilienceHandler();

            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });
    }

    public static void ConfigureRabbitMq(IBusRegistrationContext context, IRabbitMqBusFactoryConfigurator cfg)
    {
        var applicationOptions = context.GetRequiredService<IOptions<ApplicationOptions>>();
        var configService = context.GetRequiredService<IConfiguration>();
        var connectionString = configService.GetConnectionString("AMQPConnectionString");
        var hasConnectionString = !string.IsNullOrWhiteSpace(connectionString);

        if (applicationOptions != null && hasConnectionString)
        {
            cfg.Host(connectionString);
        }
    }

    private static void ConfigureAspireLogging(this ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });
    }

    private static IHostApplicationBuilder ConfigureAspireOpenTelemetry(this IHostApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        builder.AddOpenTelemetryExporters();

        return builder;
    }

    public static IHostBuilder ConfigureAspireOpenTelemetry(this IHostBuilder builder, HostBuilderContext hostContext, IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    // Uncomment the following line to enable gRPC instrumentation (requires the OpenTelemetry.Instrumentation.GrpcNetClient package)
                    //.AddGrpcClientInstrumentation()
                    .AddHttpClientInstrumentation();
            });

        AddOpenTelemetryExporters(hostContext, services);

        return builder;
    }

    private static IHostApplicationBuilder AddOpenTelemetryExporters(this IHostApplicationBuilder builder)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            builder.Services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    private static void AddOpenTelemetryExporters(HostBuilderContext hostContext, IServiceCollection services)
    {
        var useOtlpExporter = !string.IsNullOrWhiteSpace(hostContext.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);

        if (useOtlpExporter)
        {
            services.AddOpenTelemetry().UseOtlpExporter();
        }

        // Uncomment the following lines to enable the Azure Monitor exporter (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
        //if (!string.IsNullOrEmpty(hostContext.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //  services.AddOpenTelemetry()
        //      .UseAzureMonitor();
        //}
    }

    public static IHostApplicationBuilder AddDefaultHealthChecks(this IHostApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);

        return builder;
    }

    public static void AddDefaultHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            // Add a default liveness check to ensure app is responsive
            .AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // Adding health checks endpoints to applications in non-development environments has security implications.
        // See https://aka.ms/dotnet/aspire/healthchecks for details before enabling these endpoints in non-development environments.
        if (app.Environment.IsDevelopment())
        {
            // All health checks must pass for app to be considered ready to accept traffic after starting
            app.MapHealthChecks("/health");

            // Only health checks tagged with the "live" tag must pass for app to be considered alive
            app.MapHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = r => r.Tags.Contains("live")
            });
        }

        return app;
    }
}