namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class WindmillResourceBuilderExtensions
{
    public static IResourceBuilder<WindmillResource> AddWindmill(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "windmill",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new WindmillResource(name);

        return builder.AddResource(resource)
                      .WithImage(WindmillContainerImageTags.Image)
                      .WithImageRegistry(WindmillContainerImageTags.Registry)
                      .WithImageTag(WindmillContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 8000,
                          port: httpPort,
                          name: WindmillResource.HttpEndpointName)
                      .WithEnvironment("JSON_FMT", "true")
                      .WithEnvironment("NUM_WORKERS", "3");
    }

    public static IResourceBuilder<WindmillResource> WithDataVolume(this IResourceBuilder<WindmillResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<WindmillResource> windmillResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(windmillResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{windmillResource.Resource.Name}"] = windmillResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class WindmillContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "windmill-labs/windmill";

    internal const string Tag = "latest";
}
