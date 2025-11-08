// For ease of discovery, resource types should be placed in
// the Aspire.Hosting.ApplicationModel namespace. If there is
// likelihood of a conflict on the resource name consider using
// an alternative namespace.
namespace Aspire.Hosting.ApplicationModel;

public sealed class FlowiseResource(
    string name,
    ParameterResource? apiKeyParameterResource = null,
    ParameterResource? usernameParameterResource = null,
    ParameterResource? passwordParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    // Constants used to refer to well known-endpoint names, this is specific
    // for each resource type. Flowise exposes an HTTP endpoint.
    internal const string HttpEndpointName = "http";

    public ReferenceExpression ConnectionStringExpression
        => (apiKeyParameterResource, usernameParameterResource, passwordParameterResource) switch
        {
            (ParameterResource apiKey, _, _) => ReferenceExpression.Create($"Endpoint={new EndpointReference(this, HttpEndpointName).Property(EndpointProperty.HostAndPort)};Key={apiKey.ValueExpression}"),
            (_, ParameterResource username, ParameterResource password) => ReferenceExpression.Create($"Endpoint={new EndpointReference(this, HttpEndpointName).Property(EndpointProperty.HostAndPort)};Username={username.ValueExpression};Password={password.ValueExpression}"),
            _ => ReferenceExpression.Create($"Endpoint={new EndpointReference(this, HttpEndpointName).Property(EndpointProperty.HostAndPort)}"),
        };
}