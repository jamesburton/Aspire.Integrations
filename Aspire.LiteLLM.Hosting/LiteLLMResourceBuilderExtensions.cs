namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class LiteLLMResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="LiteLLMResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="masterKeyParameter">The master key parameter for authentication.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{LiteLLMResource}"/> instance that
    /// represents the added LiteLLM resource.
    /// </returns>
    public static IResourceBuilder<LiteLLMResource> AddLiteLLM(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "litellm",
        int? httpPort = null,
        ParameterResource? masterKeyParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new LiteLLMResource(name, masterKeyParameter);

        var litellm = builder.AddResource(resource)
                          .WithImage(LiteLLMContainerImageTags.Image)
                          .WithImageRegistry(LiteLLMContainerImageTags.Registry)
                          .WithImageTag(LiteLLMContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 4000,
                              port: httpPort,
                              name: LiteLLMResource.HttpEndpointName)
                          .WithEnvironment("STORE_MODEL_IN_DB", "True");

        if (masterKeyParameter != null)
            litellm.WithEnvironment("LITELLM_MASTER_KEY", masterKeyParameter);

        return litellm;
    }

    /// <summary>Adds a named volume for the data folder to a LiteLLM container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<LiteLLMResource> WithDataVolume(this IResourceBuilder<LiteLLMResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the data folder to a LiteLLM container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<LiteLLMResource> WithDataBindMount(this IResourceBuilder<LiteLLMResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/app/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a LiteLLM server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="liteLLMResource">The LiteLLM server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<LiteLLMResource> liteLLMResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(liteLLMResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{liteLLMResource.Resource.Name}"] = liteLLMResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class LiteLLMContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "berriai/litellm";

    internal const string Tag = "main-stable";
}
