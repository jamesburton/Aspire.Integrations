namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class OpenObserveResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="OpenObserveResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="adminEmail">The admin email address for the root user.</param>
    /// <param name="passwordParameter">The password parameter for the root user.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{OpenObserveResource}"/> instance that
    /// represents the added OpenObserve resource.
    /// </returns>
    public static IResourceBuilder<OpenObserveResource> AddOpenObserve(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "openobserve",
        int? httpPort = null,
        string adminEmail = "admin@example.com",
        ParameterResource? passwordParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new OpenObserveResource(name, passwordParameter);

        var openObserve = builder.AddResource(resource)
                              .WithImage(OpenObserveContainerImageTags.Image)
                              .WithImageRegistry(OpenObserveContainerImageTags.Registry)
                              .WithImageTag(OpenObserveContainerImageTags.Tag)
                              .WithHttpEndpoint(
                                  targetPort: 5080,
                                  port: httpPort,
                                  name: OpenObserveResource.HttpEndpointName)
                              .WithEnvironment("ZO_ROOT_USER_EMAIL", adminEmail);

        if (passwordParameter != null)
            openObserve.WithEnvironment("ZO_ROOT_USER_PASSWORD", passwordParameter);

        return openObserve;
    }

    /// <summary>Adds a named volume for the data folder to an OpenObserve container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<OpenObserveResource> WithDataVolume(this IResourceBuilder<OpenObserveResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to an OpenObserve server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="openObserveResource">The OpenObserve server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<OpenObserveResource> openObserveResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(openObserveResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{openObserveResource.Resource.Name}"] = openObserveResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class OpenObserveContainerImageTags
{
    internal const string Registry = "public.ecr.aws";

    internal const string Image = "zinclabs/openobserve";

    internal const string Tag = "latest";
}
