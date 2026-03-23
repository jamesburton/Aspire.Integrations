namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class OpenWebUIResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="OpenWebUIResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{OpenWebUIResource}"/> instance that
    /// represents the added Open WebUI resource.
    /// </returns>
    public static IResourceBuilder<OpenWebUIResource> AddOpenWebUI(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "open-webui",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new OpenWebUIResource(name);

        return builder.AddResource(resource)
                      .WithImage(OpenWebUIContainerImageTags.Image)
                      .WithImageRegistry(OpenWebUIContainerImageTags.Registry)
                      .WithImageTag(OpenWebUIContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 8080,
                          port: httpPort,
                          name: OpenWebUIResource.HttpEndpointName);
    }

    /// <summary>
    /// Configures the Open WebUI resource to use an Ollama backend.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="ollamaEndpoint">The base URL of the Ollama API endpoint.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<OpenWebUIResource> WithOllamaReference(
        this IResourceBuilder<OpenWebUIResource> builder,
        string ollamaEndpoint)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(ollamaEndpoint);

        return builder.WithEnvironment("OLLAMA_BASE_URL", ollamaEndpoint);
    }

    /// <summary>
    /// Configures the Open WebUI resource to use an OpenAI-compatible endpoint.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="baseUrl">The base URL of the OpenAI-compatible API.</param>
    /// <param name="apiKey">The API key for authentication.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<OpenWebUIResource> WithOpenAIEndpoint(
        this IResourceBuilder<OpenWebUIResource> builder,
        string baseUrl,
        string apiKey)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(baseUrl);
        ArgumentException.ThrowIfNullOrEmpty(apiKey);

        return builder
            .WithEnvironment("OPENAI_API_BASE_URL", baseUrl)
            .WithEnvironment("OPENAI_API_KEY", apiKey);
    }

    /// <summary>Adds a named volume for the data folder to an Open WebUI container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<OpenWebUIResource> WithDataVolume(
        this IResourceBuilder<OpenWebUIResource> builder,
        string? name = null,
        bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/backend/data",
            isReadOnly);
    }

    /// <summary>
    /// Add a reference to an Open WebUI server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="openWebUIResource">The Open WebUI server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<OpenWebUIResource> openWebUIResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(openWebUIResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{openWebUIResource.Resource.Name}"] = openWebUIResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class OpenWebUIContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "open-webui/open-webui";

    internal const string Tag = "main";
}
