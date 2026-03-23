using Flowise.Client;
using Flowise.Client.Models;

namespace Aspire.Flowise.Client;

/// <summary>
/// A factory for creating <see cref="IFlowiseClient"/> instances.
/// </summary>
/// <param name="settings">
/// The <see cref="FlowiseClientSettings"/> settings for the Flowise API server.
/// </param>
public sealed class FlowiseClientFactory(FlowiseClientSettings settings)
{
    /// <summary>
    /// Gets an <see cref="IFlowiseClient"/> instance configured with the current settings.
    /// </summary>
    /// <returns>A configured <see cref="IFlowiseClient"/> instance.</returns>
    public IFlowiseClient GetFlowiseClient() => new FlowiseClient(new FlowiseClientOptions
    {
        BaseUrl = settings.Endpoint?.ToString() ?? string.Empty,
        ApiKey = settings.ApiKey,
    });
}
