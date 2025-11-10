using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Aspire.N8N.Client;

public static class N8NClientExtensions
{
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
            $"{N8NClientSettings.DefaultConfigSectionName}:{name}",
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

        N8NClientFactory CreateFactory(IServiceProvider _)
            => new N8NClientFactory(settings);

        if (serviceKey is null)
        {
            builder.Services.AddScoped(CreateFactory);
            builder.Services.AddScoped(sp => sp.GetRequiredService<N8NClientFactory>().GetN8NClient());
        }
        else
        {
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) => CreateFactory(sp));
            builder.Services.AddKeyedScoped(serviceKey, (sp, key) => sp.GetRequiredService<N8NClientFactory>().GetN8NClient());
        }

        if (!settings.DisableHealthChecks)
        {
            // Optional health checks can be added later.
        }
    }
}
