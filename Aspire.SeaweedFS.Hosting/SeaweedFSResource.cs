namespace Aspire.Hosting.ApplicationModel;

public sealed class SeaweedFSResource(string name)
    : ContainerResource(name), IResourceWithConnectionString
{
    internal const string S3EndpointName = "s3";
    internal const string HttpEndpointName = "http";

    private EndpointReference? _s3Reference;

    public EndpointReference S3Endpoint => _s3Reference ??= new(this, S3EndpointName);

    public ReferenceExpression ConnectionStringExpression =>
        ReferenceExpression.Create($"{S3Endpoint.Property(EndpointProperty.Url)}");
}
