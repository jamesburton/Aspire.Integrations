namespace Aspire.Hosting.ApplicationModel;

public sealed class GiteaResource(string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";
    internal const string SshEndpointName = "ssh";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}");
}
