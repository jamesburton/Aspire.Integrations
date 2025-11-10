using System;
using N8N.Client;
using N8N.Client.Models;

namespace Aspire.N8N.Client;

/// <summary>
/// Factory for creating configured <see cref="IN8NClient"/> instances.
/// </summary>
public sealed class N8NClientFactory
{
    private readonly N8NClientSettings _settings;

    public N8NClientFactory(N8NClientSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public IN8NClient GetN8NClient()
    {
        if (_settings.Endpoint is null)
            throw new InvalidOperationException("N8N endpoint not configured.");
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new InvalidOperationException("N8N API key not configured.");

        return new N8NClient(new N8NClientOptions
        {
            BaseUrl = _settings.Endpoint.ToString().TrimEnd('/'),
            ApiKey = _settings.ApiKey!,
            TimeoutMilliseconds = _settings.TimeoutMilliseconds
        });
    }
}
