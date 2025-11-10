using Microsoft.Extensions.Diagnostics.HealthChecks;
using N8N.Client.Models;

namespace Aspire.N8N.Client;

internal sealed class N8NHealthCheck(N8NClientFactory factory) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // The factory connects (and authenticates).
            using var client = factory.GetN8NClient();

            // Try to get workflows to verify connectivity and authentication
            var workflows = await client.GetWorkflowsAsync(
                new ListWorkflowsOptions { Limit = 1 },
                cancellationToken).ConfigureAwait(false);

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(exception: ex);
        }
    }
}
