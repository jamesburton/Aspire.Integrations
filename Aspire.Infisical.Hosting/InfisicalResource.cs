namespace Aspire.Hosting.ApplicationModel;

public sealed class InfisicalResource(
    string name,
    ParameterResource? encryptionKeyParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        encryptionKeyParameterResource switch
        {
            ParameterResource encryptionKey => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)};Key={encryptionKey}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}")
        };
}
