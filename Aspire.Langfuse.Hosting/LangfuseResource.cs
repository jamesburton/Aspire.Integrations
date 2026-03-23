namespace Aspire.Hosting.ApplicationModel;

public sealed class LangfuseResource(
    string name,
    ParameterResource? secretKeyParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        secretKeyParameterResource switch
        {
            ParameterResource secretKey => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)};SecretKey={secretKey}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}")
        };
}
