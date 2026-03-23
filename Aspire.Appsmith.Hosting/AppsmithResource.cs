namespace Aspire.Hosting.ApplicationModel;

public sealed class AppsmithResource(
    string name,
    ParameterResource? encryptionPasswordParameterResource = null)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";

    private EndpointReference? _httpReference;

    public EndpointReference HttpEndpoint => _httpReference ??= new(this, HttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        encryptionPasswordParameterResource switch
        {
            ParameterResource password => ReferenceExpression.Create($"Endpoint={HttpEndpoint.Property(EndpointProperty.Url)};Password={password}"),
            _ => ReferenceExpression.Create($"{HttpEndpoint.Property(EndpointProperty.Url)}")
        };
}
