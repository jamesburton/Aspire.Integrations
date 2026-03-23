namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class PlaneResourceBuilderExtensions
{
    public static IResourceBuilder<PlaneResource> AddPlane(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "plane",
        int? httpPort = null,
        ParameterResource? secretKeyParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new PlaneResource(name, secretKeyParameter);

        var plane = builder.AddResource(resource)
                       .WithImage(PlaneContainerImageTags.Image)
                       .WithImageRegistry(PlaneContainerImageTags.Registry)
                       .WithImageTag(PlaneContainerImageTags.Tag)
                       .WithHttpEndpoint(
                           targetPort: 80,
                           port: httpPort,
                           name: PlaneResource.HttpEndpointName);

        if (secretKeyParameter != null)
            plane.WithEnvironment("SECRET_KEY", secretKeyParameter);

        return plane;
    }

    public static IResourceBuilder<PlaneResource> WithDataVolume(this IResourceBuilder<PlaneResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<PlaneResource> planeResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(planeResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{planeResource.Resource.Name}"] = planeResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class PlaneContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "makeplane/plane-aio-community";

    internal const string Tag = "latest";
}
