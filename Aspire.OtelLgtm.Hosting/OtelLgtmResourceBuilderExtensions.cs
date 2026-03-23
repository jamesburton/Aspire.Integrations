namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class OtelLgtmResourceBuilderExtensions
{
    public static IResourceBuilder<OtelLgtmResource> AddOtelLgtm(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "otel-lgtm",
        int? grafanaPort = null,
        int? otlpGrpcPort = null,
        int? otlpHttpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new OtelLgtmResource(name);

        return builder.AddResource(resource)
            .WithImage(OtelLgtmContainerImageTags.Image)
            .WithImageRegistry(OtelLgtmContainerImageTags.Registry)
            .WithImageTag(OtelLgtmContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: 3000,
                port: grafanaPort,
                name: OtelLgtmResource.HttpEndpointName)
            .WithEndpoint(
                targetPort: 4317,
                port: otlpGrpcPort,
                name: OtelLgtmResource.OtlpGrpcEndpointName,
                scheme: "http")
            .WithEndpoint(
                targetPort: 4318,
                port: otlpHttpPort,
                name: OtelLgtmResource.OtlpHttpEndpointName,
                scheme: "http")
            .WithDataVolume();
    }

    public static IResourceBuilder<OtelLgtmResource> WithDataVolume(
        this IResourceBuilder<OtelLgtmResource> builder,
        string? name = null,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<OtelLgtmResource> otelLgtmResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(otelLgtmResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{otelLgtmResource.Resource.Name}"] =
                otelLgtmResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class OtelLgtmContainerImageTags
{
    internal const string Registry = "docker.io";
    internal const string Image = "grafana/otel-lgtm";
    internal const string Tag = "latest";
}
