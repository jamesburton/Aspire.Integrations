namespace Aspire.Hosting.ApplicationModel;

public sealed class GrafanaResource(
    string name,
    ParameterResource? adminPasswordParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        adminPasswordParameterResource switch
        {
            ParameterResource adminPassword => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)};Password={adminPassword}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}")
        };
}
