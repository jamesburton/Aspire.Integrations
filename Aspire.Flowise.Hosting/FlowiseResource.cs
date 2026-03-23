namespace Aspire.Hosting.ApplicationModel;

public sealed class FlowiseResource(
    string name,
    ParameterResource? apiKeyParameterResource = null,
    ParameterResource? usernameParameterResource = null,
    ParameterResource? passwordParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        (apiKeyParameterResource, usernameParameterResource, passwordParameterResource) switch
        {
            (ParameterResource apiKey, _, _) => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)}/api/v1/;Key={apiKey}"),
            (_, ParameterResource username, ParameterResource password) => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)}/api/v1/;Username={username};Password={password}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}/api/v1/"),
        };
}
