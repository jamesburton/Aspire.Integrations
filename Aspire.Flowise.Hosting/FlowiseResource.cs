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

    // An EndpointReference is a core Aspire type used for keeping
    // track of endpoint details in expressions. Simple literal values cannot
    // be used because endpoints are not known until containers are launched.
    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    // Required property on IResourceWithConnectionString. Represents a connection
    // string that applications can use to access the MailDev server. In this case
    // the connection string is composed of the HttpEndpoint endpoint reference.
    public ReferenceExpression ConnectionStringExpression =>
        (apiKeyParameterResource, usernameParameterResource, passwordParameterResource) switch
        {
            // (ParameterResource apiKey, _, _) => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Scheme}://{HttpEndpoint.Property(EndpointProperty.HostAndPort)};Key={apiKey.Value}"),
            (ParameterResource apiKey, _, _) => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)}/api/v1/;Key={apiKey.Value}"),
            // (_, ParameterResource username, ParameterResource password) => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Scheme}://{HttpEndpoint.Property(EndpointProperty.HostAndPort)};Username={username.Value};Password={password.Value}"),
            (_, ParameterResource username, ParameterResource password) => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)}/api/v1/;Username={username.Value};Password={password.Value}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}/api/v1/"),
        };
}