namespace Aspire.Hosting.ApplicationModel;

public sealed class MemcachedResource(string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string MemcachedEndpointName = "memcached";

    private EndpointReference? _memcachedReference;

    public EndpointReference MemcachedEndpoint => _memcachedReference ??= new(this, MemcachedEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{MemcachedEndpoint.Property(EndpointProperty.HostAndPort)}");
}
