namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class N8NResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="N8NResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="apiKeyParameter">The API key parameter for authentication.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{N8NResource}"/> instance that
    /// represents the added N8N resource.
    /// </returns>
    public static IResourceBuilder<N8NResource> AddN8N(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "n8n",
        int? httpPort = null,
        ParameterResource? apiKeyParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new N8NResource(name, apiKeyParameter);

        var n8n = builder.AddResource(resource)
                      .WithImage(N8NContainerImageTags.Image)
                      .WithImageRegistry(N8NContainerImageTags.Registry)
                      .WithImageTag(N8NContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 5678,
                          port: httpPort,
                          name: N8NResource.HttpEndpointName)
                      .WithEnvironment("N8N_HOST", "0.0.0.0")
                      .WithEnvironment("N8N_PORT", "5678")
                      .WithEnvironment("WEBHOOK_URL", "http://localhost:5678/");

        if (apiKeyParameter != null)
            n8n.WithEnvironment("N8N_API_KEY", apiKeyParameter);

        return n8n;
    }

    /// <summary>Adds a named volume for the data folder to an N8N container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<N8NResource> WithDataVolume(this IResourceBuilder<N8NResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/home/node/.n8n",
            isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the data folder to an N8N container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<N8NResource> WithDataBindMount(this IResourceBuilder<N8NResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/home/node/.n8n", isReadOnly);
    }

    /// <summary>
    /// Add a reference to an N8N server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="n8nResource">The N8N server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<N8NResource> n8nResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(n8nResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{n8nResource.Resource.Name}"] = n8nResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class N8NContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "n8nio/n8n";

    internal const string Tag = "latest";
}
