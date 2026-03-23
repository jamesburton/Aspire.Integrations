namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class LangfuseResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="LangfuseResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="secretKeyParameter">The secret key parameter for authentication.</param>
    /// <param name="nextAuthSecret">The NextAuth secret parameter.</param>
    /// <param name="salt">The salt parameter.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{LangfuseResource}"/> instance that
    /// represents the added Langfuse resource.
    /// </returns>
    public static IResourceBuilder<LangfuseResource> AddLangfuse(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "langfuse",
        int? httpPort = null,
        ParameterResource? secretKeyParameter = null,
        ParameterResource? nextAuthSecret = null,
        ParameterResource? salt = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new LangfuseResource(name, secretKeyParameter);

        var langfuse = builder.AddResource(resource)
                          .WithImage(LangfuseContainerImageTags.Image)
                          .WithImageRegistry(LangfuseContainerImageTags.Registry)
                          .WithImageTag(LangfuseContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 3000,
                              port: httpPort,
                              name: LangfuseResource.HttpEndpointName)
                          .WithEnvironment(context =>
                          {
                              context.EnvironmentVariables["NEXTAUTH_URL"] = resource.HttpEndpoint.Property(EndpointProperty.Url);
                          });

        if (nextAuthSecret != null)
            langfuse.WithEnvironment("NEXTAUTH_SECRET", nextAuthSecret);

        if (salt != null)
            langfuse.WithEnvironment("SALT", salt);

        return langfuse;
    }

    /// <summary>Adds a named volume for the data folder to a Langfuse container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<LangfuseResource> WithDataVolume(this IResourceBuilder<LangfuseResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Langfuse server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="langfuseResource">The Langfuse server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<LangfuseResource> langfuseResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(langfuseResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{langfuseResource.Resource.Name}"] = langfuseResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class LangfuseContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "langfuse/langfuse";

    internal const string Tag = "3";
}
