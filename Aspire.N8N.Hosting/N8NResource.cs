namespace Aspire.Hosting.ApplicationModel;

public sealed class N8NResource(
    string name,
    ParameterResource? apiKeyParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        apiKeyParameterResource switch
        {
            ParameterResource apiKey => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)}/api/v1;Key={apiKey}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}/api/v1")
        };
}
