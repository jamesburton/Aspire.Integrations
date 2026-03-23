namespace Aspire.Hosting.ApplicationModel;

public sealed class DocmostResource(
    string name,
    ParameterResource? appSecretParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        appSecretParameterResource switch
        {
            ParameterResource appSecret => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)};Secret={appSecret}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}")
        };
}
