namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class DozzleResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="DozzleResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{DozzleResource}"/> instance that
    /// represents the added Dozzle resource.
    /// </returns>
    public static IResourceBuilder<DozzleResource> AddDozzle(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "dozzle",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new DozzleResource(name);

        return builder.AddResource(resource)
                      .WithImage(DozzleContainerImageTags.Image)
                      .WithImageRegistry(DozzleContainerImageTags.Registry)
                      .WithImageTag(DozzleContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 8080,
                          port: httpPort,
                          name: DozzleResource.HttpEndpointName)
                      .WithBindMount("/var/run/docker.sock", "/var/run/docker.sock", isReadOnly: true)
                      .WithEnvironment("DOZZLE_NO_ANALYTICS", "1");
    }

    /// <summary>
    /// Add a reference to a Dozzle server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="dozzleResource">The Dozzle server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<DozzleResource> dozzleResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(dozzleResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{dozzleResource.Resource.Name}"] = dozzleResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class DozzleContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "amir20/dozzle";

    internal const string Tag = "latest";
}
