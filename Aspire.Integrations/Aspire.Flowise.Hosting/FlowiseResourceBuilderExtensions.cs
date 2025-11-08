// Put extensions in the Aspire.Hosting namespace to ease discovery as referencing
// the Aspire hosting package automatically adds this namespace.
namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class FlowiseResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="FlowiseResource"/> to the given
    /// <paramref name="builder"/> instance. Uses the "2.1.0" tag.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="smtpPort">The SMTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{MailDevResource}"/> instance that
    /// represents the added MailDev resource.
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

        // The AddResource method is a core API within Aspire and is
        // used by resource developers to wrap a custom resource in an
        // IResourceBuilder<T> instance. Extension methods to customize
        // the resource (if any exist) target the builder interface.
        var resource = new FlowiseResource(name, apiKeyParameter, usernameParameter, passwordParameter);

        var flowise = builder.AddResource(resource)
                      .WithImage(FlowiseContainerImageTags.Image)
                      .WithImageRegistry(FlowiseContainerImageTags.Registry)
                      .WithImageTag(FlowiseContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 3000,
                          port: httpPort/*,
                          name: FlowiseResource.HttpEndpointName*/)
                      .WithEnvironment("SHOW_COMMUNITY_NODES", showCommunityNodes.ToString());

        if (secretKeyParameter != null)
            flowise.WithEnvironment("FLOWISE_SECRETKEY_OVERWRITE", secretKeyParameter!);

        if (usernameParameter != null)
            flowise.WithEnvironment("FLOWISE_USERNAME", usernameParameter!);

        if (passwordParameter != null)
            flowise.WithEnvironment("FLOWISE_PASSWORD", passwordParameter!);

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

// This class just contains constant strings that can be updated periodically
// when new versions of the underlying container are released.
internal static class FlowiseContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "flowiseai/flowise";

    internal const string Tag = "3.0.10";
}