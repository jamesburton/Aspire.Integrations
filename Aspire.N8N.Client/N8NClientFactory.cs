using N8N.Client;
using N8N.Client.Models;

namespace Aspire.N8N.Client;

/// <summary>
/// Factory for creating configured <see cref="IN8NClient"/> instances.
/// </summary>
public sealed class N8NClientFactory(N8NClientSettings settings)
{
    public IN8NClient GetN8NClient()
    {
        ArgumentNullException.ThrowIfNull(settings);

        if (settings.Endpoint is null)
            throw new InvalidOperationException("N8N endpoint not configured.");
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
            throw new InvalidOperationException("N8N API key not configured.");

        return new N8NClient(new N8NClientOptions
        {
            BaseUrl = settings.Endpoint.ToString().TrimEnd('/'),
            ApiKey = settings.ApiKey!,
            TimeoutMilliseconds = settings.TimeoutMilliseconds
        });
    }
}
