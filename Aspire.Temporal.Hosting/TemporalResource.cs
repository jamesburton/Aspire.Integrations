namespace Aspire.Hosting.ApplicationModel;

public sealed class TemporalResource(string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string GrpcEndpointName = "grpc";
    internal const string HttpEndpointName = "http";

    private EndpointReference? _grpcReference;

    public EndpointReference GrpcEndpoint => _grpcReference ??= new(this, GrpcEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{GrpcEndpoint.Property(EndpointProperty.Url)}");
}
