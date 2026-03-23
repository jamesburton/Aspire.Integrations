namespace Aspire.Hosting.ApplicationModel;

public sealed class OtelLgtmResource(string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string HttpEndpointName = "http";
    internal const string OtlpGrpcEndpointName = "otlp-grpc";
    internal const string OtlpHttpEndpointName = "otlp-http";

    private EndpointReference? _otlpGrpcReference;

    public EndpointReference HttpEndpoint => new(this, HttpEndpointName);
    public EndpointReference OtlpGrpcEndpoint => _otlpGrpcReference ??= new(this, OtlpGrpcEndpointName);
    public EndpointReference OtlpHttpEndpoint => new(this, OtlpHttpEndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{OtlpGrpcEndpoint.Property(EndpointProperty.Url)}");
}
