using Aspire.Hosting.ApplicationModel;

namespace Aspire.Hosting;

public static class FlowiseResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="FlowiseResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="apiKeyParameter">The API key parameter for authentication.</param>
    /// <param name="usernameParameter">The username parameter for authentication.</param>
    /// <param name="passwordParameter">The password parameter for authentication.</param>
    /// <param name="secretKeyParameter">The secret key parameter for encryption.</param>
    /// <param name="showCommunityNodes">Whether to show community nodes.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{FlowiseResource}"/> instance that
    /// represents the added Flowise resource.
    /// </returns>
    public static IResourceBuilder<FlowiseResource> AddFlowise(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "flowise",
        int? httpPort = null,
        ParameterResource? apiKeyParameter = null,
        ParameterResource? usernameParameter = null,
        ParameterResource? passwordParameter = null,
        ParameterResource? secretKeyParameter = null,
        bool showCommunityNodes = true)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new FlowiseResource(name, apiKeyParameter, usernameParameter, passwordParameter);

        var flowise = builder.AddResource(resource)
                      .WithImage(FlowiseContainerImageTags.Image)
                      .WithImageRegistry(FlowiseContainerImageTags.Registry)
                      .WithImageTag(FlowiseContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 3000,
                          port: httpPort)
                      .WithEnvironment("SHOW_COMMUNITY_NODES", showCommunityNodes.ToString());

        if (secretKeyParameter != null)
            flowise.WithEnvironment("FLOWISE_SECRETKEY_OVERWRITE", secretKeyParameter);

        if (usernameParameter != null)
            flowise.WithEnvironment("FLOWISE_USERNAME", usernameParameter);

        if (passwordParameter != null)
            flowise.WithEnvironment("FLOWISE_PASSWORD", passwordParameter);

        return flowise;
    }

    /// <summary>Adds a named volume for the data folder to a Flowise container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<FlowiseResource> WithDataVolume(this IResourceBuilder<FlowiseResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/root/.flowise",
            isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the data folder to a Flowise container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<FlowiseResource> WithDataBindMount(this IResourceBuilder<FlowiseResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/root/.flowise", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Flowise server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="flowiseResource">The Flowise server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<FlowiseResource> flowiseResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(flowiseResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{flowiseResource.Resource.Name}"] = flowiseResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class FlowiseContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "flowiseai/flowise";

    internal const string Tag = "3.0.10";
}