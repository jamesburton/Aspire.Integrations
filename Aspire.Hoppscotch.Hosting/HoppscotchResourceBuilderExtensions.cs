namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class HoppscotchResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="HoppscotchResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{HoppscotchResource}"/> instance that
    /// represents the added Hoppscotch resource.
    /// </returns>
    public static IResourceBuilder<HoppscotchResource> AddHoppscotch(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "hoppscotch",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new HoppscotchResource(name);

        return builder.AddResource(resource)
                      .WithImage(HoppscotchContainerImageTags.Image)
                      .WithImageRegistry(HoppscotchContainerImageTags.Registry)
                      .WithImageTag(HoppscotchContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 3000,
                          port: httpPort,
                          name: HoppscotchResource.HttpEndpointName);
    }

    /// <summary>Adds a named volume for the data folder to a Hoppscotch container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<HoppscotchResource> WithDataVolume(this IResourceBuilder<HoppscotchResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Hoppscotch server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="hoppscotchResource">The Hoppscotch server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<HoppscotchResource> hoppscotchResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(hoppscotchResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{hoppscotchResource.Resource.Name}"] = hoppscotchResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class HoppscotchContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "hoppscotch/hoppscotch";

    internal const string Tag = "latest";
}
