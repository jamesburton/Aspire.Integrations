namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class BytebaseResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="BytebaseResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{BytebaseResource}"/> instance that
    /// represents the added Bytebase resource.
    /// </returns>
    public static IResourceBuilder<BytebaseResource> AddBytebase(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "bytebase",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new BytebaseResource(name);

        return builder.AddResource(resource)
                      .WithImage(BytebaseContainerImageTags.Image)
                      .WithImageRegistry(BytebaseContainerImageTags.Registry)
                      .WithImageTag(BytebaseContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 8080,
                          port: httpPort,
                          name: BytebaseResource.HttpEndpointName)
                      .WithArgs("--data", "/var/opt/bytebase", "--port", "8080")
                      .WithDataVolume();
    }

    /// <summary>Adds a named volume for the data folder to a Bytebase container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<BytebaseResource> WithDataVolume(this IResourceBuilder<BytebaseResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/var/opt/bytebase", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Bytebase server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="bytebaseResource">The Bytebase server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<BytebaseResource> bytebaseResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(bytebaseResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{bytebaseResource.Resource.Name}"] = bytebaseResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class BytebaseContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "bytebase/bytebase";

    internal const string Tag = "latest";
}
