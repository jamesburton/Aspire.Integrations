namespace Aspire.Hosting.ApplicationModel;

public sealed class SoketiResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    internal const string WsEndpointName = "ws";

    private EndpointReference? _wsReference;

    public EndpointReference WsEndpoint => _wsReference ??= new(this, WsEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{WsEndpoint.Property(EndpointProperty.Url)}");
}
