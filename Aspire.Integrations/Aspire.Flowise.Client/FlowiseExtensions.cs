using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aspire.Flowise.Client;

public static class FlowiseExtensions
    {
    /// <summary>
    /// Registers 'Scoped' <see cref="FlowiseClientFactory" /> for creating
    /// connected <see cref="FlowiseClient"/> instance for interacting with Flowise server.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IHostApplicationBuilder" /> to read config from and add services to.
    /// </param>
    /// <param name="connectionName">
    /// A name used to retrieve the connection string from the ConnectionStrings configuration section.
    /// </param>
    /// <param name="configureSettings">
    /// An optional delegate that can be used for customizing options.
    /// It's invoked after the settings are read from the configuration.
    /// </param>
    public static void AddFlowiseClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<FlowiseClientSettings>? configureSettings = null)
        => AddFlowiseClient(
            builder,
            FlowiseClientSettings.DefaultConfigSectionName,
            configureSettings,
            connectionName,
            serviceKey: null);

    public static void AddKeyedFlowiseClient(
        this IHostApplicationBuilder builder,
        string name,
        Action<FlowiseClientSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        AddFlowiseClient(
            builder,
            FlowiseClientSettings.DefaultConfigSectionName,
            configureSettings,
            connectionName: name,
            serviceKey: name);
    }

    public static void AddFlowiseClient(
        this IHostApplicationBuilder builder,
        string configSectionName,
        Action<FlowiseClientSettings>? configureSettings,
        string connectionName,
        string? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new FlowiseClientSettings();

        builder.Configuration
               .GetSection(configSectionName)
               .Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
            settings.ParseConnectionString(connectionString);

        configureSettings?.Invoke(settings);

        FlowiseClientFactory CreateFlowiseClientFactory(IServiceProvider _)
            => new FlowiseClientFactory(settings);

        if (serviceKey is null)
        {
            builder.Services.AddScoped(CreateFlowiseClientFactory);
            builder.Services.AddScoped(sp => sp.GetRequiredService<FlowiseClientFactory>().GetFlowiseClient());
        }
        else
        {
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) => CreateFlowiseClientFactory(sp));
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) =>
                sp.GetRequiredService<FlowiseClientFactory>().GetFlowiseClient());
        }

        if (!settings.DisableHealthChecks)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<FlowiseHealthCheck>(
                    name: serviceKey is null ? "Flowise" : $"Flowise_{connectionName}",
                    failureStatus: default,
                    tags: []);
        }

        //if (!settings.DisableTracing)
        //{
        //    builder.Services.AddOpenTelemetry()
        //        .WithTracing(
        //            traceBuilder => traceBuilder.AddSource(
        //                Telemetry.SmtpClient.ActivitySourceName));
        //}

        //if (!settings.DisableMetrics)
        //{
        //    // Required by Flowise to enable metrics
        //    Telemetry.FlowiseClient.Configure();

        //    builder.Services.AddOpenTelemetry()
        //        .WithMetrics(
        //            metricsBuilder => metricsBuilder.AddMeter(
        //                Telemetry.FlowiseClient.MeterName));
        //}
    }
}