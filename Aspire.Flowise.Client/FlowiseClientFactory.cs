using Flowise.Client;
using Flowise.Client.Models;

namespace Aspire.Flowise.Client;

/// <summary>
/// A factory for creating <see cref="IFlowiseClient"/> instances
/// given a <paramref name="Uri"/> (and optional <paramref name="credentials"/>).
/// </summary>
/// <param name="settings">
/// The <see cref="FlowiseClientSettings"/> settings for the Flowise API server
/// </param>
public sealed class FlowiseClientFactory(FlowiseClientSettings settings) //: IDisposable
{

    /// <summary>
    /// Gets an <see cref="IFlowiseClient"/> instance in the connected state
    /// (and that's been authenticated if configured).
    /// </summary>
    /// <returns>A connected (and authenticated) <see cref="IFlowiseClient"/> instance.</returns>
    /// <remarks>
    /// Since both the connection and authentication are considered expensive operations,
    /// the <see cref="IFlowiseClient"/> returned is intended to be used for the duration of a request
    /// (registered as 'Scoped') and is automatically disposed of.
    /// </remarks>
    public IFlowiseClient GetFlowiseClient()
    {
        var options = new FlowiseClientOptions
        {
            BaseUrl = settings.Endpoint?.ToString() ?? string.Empty,
            ApiKey = settings.ApiKey,
        };

        var client = new FlowiseClient(options);

        return client;
    }
}