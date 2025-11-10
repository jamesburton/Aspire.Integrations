using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Flowise.Client;

internal sealed class FlowiseHealthCheck(FlowiseClientFactory factory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // The factory connects (and authenticates).
            using var client = factory.GetFlowiseClient();

            var result = await client.PingAsync(cancellationToken).ConfigureAwait(false);

            return result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("PingAsync failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}