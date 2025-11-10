using System;

namespace Aspire.N8N.Client;

/// <summary>
/// Settings used to configure the N8N client from configuration / connection string.
/// </summary>
public sealed class N8NClientSettings
{
    internal const string DefaultConfigSectionName = "N8N:Client";

    /// <summary>Base endpoint for the N8N server (e.g. http://localhost:5678)</summary>
    public Uri? Endpoint { get; set; }

    /// <summary>API key for authenticating with the N8N server.</summary>
    public string? ApiKey { get; set; }

    /// <summary>Timeout (ms) for API calls.</summary>
    public int TimeoutMilliseconds { get; set; } = 30000;

    /// <summary>If set to true, health checks will not be registered.</summary>
    public bool DisableHealthChecks { get; set; }

    internal void ParseConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return;

        // Supported formats:
        // 1. http://host:port
        // 2. Endpoint=http://host:port;Key=apikey
        // 3. Endpoint=http://host:port
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            if (part.Contains('=') && part.StartsWith("Endpoint=", StringComparison.OrdinalIgnoreCase))
            {
                var value = part[("Endpoint=".Length)..];
                if (Uri.TryCreate(value, UriKind.Absolute, out var uri))
                    Endpoint = uri;
            }
            else if (part.Contains('=') && part.StartsWith("Key=", StringComparison.OrdinalIgnoreCase))
            {
                ApiKey = part[("Key=".Length)..];
            }
            else if (Endpoint is null && Uri.TryCreate(part, UriKind.Absolute, out var simpleUri))
            {
                Endpoint = simpleUri;
            }
        }
    }
}
