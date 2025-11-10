using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using N8N.Client;

namespace Aspire.N8N.Client;

public static class N8NExtensions
{
    /// <summary>
    /// Registers 'Scoped' <see cref="N8NClientFactory" /> for creating
    /// connected <see cref="N8NClient"/> instance for interacting with N8N server.
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
    public static void AddN8NClient(
        this IHostApplicationBuilder builder,
        string connectionName,
        Action<N8NClientSettings>? configureSettings = null)
        => AddN8NClient(
            builder,
            N8NClientSettings.DefaultConfigSectionName,
            configureSettings,
            connectionName,
            serviceKey: null);

    public static void AddKeyedN8NClient(
        this IHostApplicationBuilder builder,
        string name,
        Action<N8NClientSettings>? configureSettings = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        AddN8NClient(
            builder,
            N8NClientSettings.DefaultConfigSectionName,
            configureSettings,
            connectionName: name,
            serviceKey: name);
    }

    public static void AddN8NClient(
        this IHostApplicationBuilder builder,
        string configSectionName,
        Action<N8NClientSettings>? configureSettings,
        string connectionName,
        string? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var settings = new N8NClientSettings();

        builder.Configuration
               .GetSection(configSectionName)
               .Bind(settings);

        if (builder.Configuration.GetConnectionString(connectionName) is string connectionString)
            settings.ParseConnectionString(connectionString);

        configureSettings?.Invoke(settings);

        N8NClientFactory CreateN8NClientFactory(IServiceProvider _)
            => new N8NClientFactory(settings);

        if (serviceKey is null)
        {
            builder.Services.AddScoped(CreateN8NClientFactory);
            builder.Services.AddScoped(sp => sp.GetRequiredService<N8NClientFactory>().GetN8NClient());
        }
        else
        {
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) => CreateN8NClientFactory(sp));
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) =>
                sp.GetRequiredService<N8NClientFactory>().GetN8NClient());
        }

        if (!settings.DisableHealthChecks)
        {
            builder.Services.AddHealthChecks()
                .AddCheck<N8NHealthCheck>(
                    name: serviceKey is null ? "N8N" : $"N8N_{connectionName}",
                    failureStatus: default,
                    tags: []);
        }

        //if (!settings.DisableTracing)
        //{
        //    builder.Services.AddOpenTelemetry()
        //        .WithTracing(
        //            traceBuilder => traceBuilder.AddSource(
        //                Telemetry.N8NClient.ActivitySourceName));
        //}

        //if (!settings.DisableMetrics)
        //{
        //    // Required by N8N to enable metrics
        //    Telemetry.N8NClient.Configure();

        //    builder.Services.AddOpenTelemetry()
        //        .WithMetrics(
        //            metricsBuilder => metricsBuilder.AddMeter(
        //                Telemetry.N8NClient.MeterName));
        //}
    }
}
