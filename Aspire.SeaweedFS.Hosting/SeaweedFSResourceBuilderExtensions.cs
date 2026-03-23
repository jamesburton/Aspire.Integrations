namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class SeaweedFSResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="SeaweedFSResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="s3Port">The S3 API port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{SeaweedFSResource}"/> instance that
    /// represents the added SeaweedFS resource.
    /// </returns>
    public static IResourceBuilder<SeaweedFSResource> AddSeaweedFS(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "seaweedfs",
        int? s3Port = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new SeaweedFSResource(name);

        return builder.AddResource(resource)
                      .WithImage(SeaweedFSContainerImageTags.Image)
                      .WithImageRegistry(SeaweedFSContainerImageTags.Registry)
                      .WithImageTag(SeaweedFSContainerImageTags.Tag)
                      .WithArgs("server", "-s3")
                      .WithEndpoint(
                          targetPort: 8333,
                          port: s3Port,
                          name: SeaweedFSResource.S3EndpointName,
                          scheme: "http")
                      .WithHttpEndpoint(
                          targetPort: 9333,
                          name: SeaweedFSResource.HttpEndpointName);
    }

    /// <summary>Adds a named volume for the data folder to a SeaweedFS container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<SeaweedFSResource> WithDataVolume(this IResourceBuilder<SeaweedFSResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a SeaweedFS server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="seaweedFSResource">The SeaweedFS server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<SeaweedFSResource> seaweedFSResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(seaweedFSResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{seaweedFSResource.Resource.Name}"] = seaweedFSResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class SeaweedFSContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "chrislusf/seaweedfs";

    internal const string Tag = "latest";
}
