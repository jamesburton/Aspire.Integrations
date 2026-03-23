namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class TemporalResourceBuilderExtensions
{
    public static IResourceBuilder<TemporalResource> AddTemporal(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "temporal",
        int? grpcPort = null,
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new TemporalResource(name);

        return builder.AddResource(resource)
            .WithImage(TemporalContainerImageTags.Image)
            .WithImageRegistry(TemporalContainerImageTags.Registry)
            .WithImageTag(TemporalContainerImageTags.Tag)
            .WithEndpoint(
                targetPort: 7233,
                port: grpcPort,
                name: TemporalResource.GrpcEndpointName,
                scheme: "http")
            .WithHttpEndpoint(
                targetPort: 8233,
                port: httpPort,
                name: TemporalResource.HttpEndpointName)
            .WithEnvironment("DB", "sqlite")
            .WithEnvironment("DEFAULT_NAMESPACE", "default");
    }

    public static IResourceBuilder<TemporalResource> WithDataVolume(
        this IResourceBuilder<TemporalResource> builder,
        string? name = null,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/etc/temporal", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<TemporalResource> temporalResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(temporalResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{temporalResource.Resource.Name}"] =
                temporalResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class TemporalContainerImageTags
{
    internal const string Registry = "docker.io";
    internal const string Image = "temporalio/auto-setup";
    internal const string Tag = "latest";
}
