namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class MeilisearchResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="MeilisearchResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="masterKeyParameter">The master key parameter for authentication.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{MeilisearchResource}"/> instance that
    /// represents the added Meilisearch resource.
    /// </returns>
    public static IResourceBuilder<MeilisearchResource> AddMeilisearch(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "meilisearch",
        int? httpPort = null,
        ParameterResource? masterKeyParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new MeilisearchResource(name, masterKeyParameter);

        var meilisearch = builder.AddResource(resource)
                      .WithImage(MeilisearchContainerImageTags.Image)
                      .WithImageRegistry(MeilisearchContainerImageTags.Registry)
                      .WithImageTag(MeilisearchContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 7700,
                          port: httpPort,
                          name: MeilisearchResource.HttpEndpointName)
                      .WithEnvironment("MEILI_ENV", "development");

        if (masterKeyParameter != null)
            meilisearch.WithEnvironment("MEILI_MASTER_KEY", masterKeyParameter);

        return meilisearch;
    }

    /// <summary>Adds a named volume for the data folder to a Meilisearch container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<MeilisearchResource> WithDataVolume(this IResourceBuilder<MeilisearchResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/meili_data", isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the data folder to a Meilisearch container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<MeilisearchResource> WithDataBindMount(this IResourceBuilder<MeilisearchResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/meili_data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Meilisearch server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="meilisearchResource">The Meilisearch server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<MeilisearchResource> meilisearchResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(meilisearchResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{meilisearchResource.Resource.Name}"] = meilisearchResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class MeilisearchContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "getmeili/meilisearch";

    internal const string Tag = "v1.12";
}
