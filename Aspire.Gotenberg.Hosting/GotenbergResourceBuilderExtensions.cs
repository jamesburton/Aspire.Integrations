namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class GotenbergResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="GotenbergResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{GotenbergResource}"/> instance that
    /// represents the added Gotenberg resource.
    /// </returns>
    public static IResourceBuilder<GotenbergResource> AddGotenberg(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "gotenberg",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new GotenbergResource(name);

        return builder.AddResource(resource)
                      .WithImage(GotenbergContainerImageTags.Image)
                      .WithImageRegistry(GotenbergContainerImageTags.Registry)
                      .WithImageTag(GotenbergContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 3000,
                          port: httpPort,
                          name: GotenbergResource.HttpEndpointName);
    }

    /// <summary>
    /// Add a reference to a Gotenberg server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="gotenbergResource">The Gotenberg server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<GotenbergResource> gotenbergResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(gotenbergResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{gotenbergResource.Resource.Name}"] = gotenbergResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class GotenbergContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "gotenberg/gotenberg";

    internal const string Tag = "8";
}
