namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class DirectusResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="DirectusResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="secretParameter">The secret parameter used by Directus for token signing.</param>
    /// <param name="adminEmail">The admin email address.</param>
    /// <param name="adminPasswordParameter">The admin password parameter.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{DirectusResource}"/> instance that
    /// represents the added Directus resource.
    /// </returns>
    public static IResourceBuilder<DirectusResource> AddDirectus(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "directus",
        int? httpPort = null,
        ParameterResource? secretParameter = null,
        string adminEmail = "admin@example.com",
        ParameterResource? adminPasswordParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new DirectusResource(name, secretParameter);

        var directus = builder.AddResource(resource)
                          .WithImage(DirectusContainerImageTags.Image)
                          .WithImageRegistry(DirectusContainerImageTags.Registry)
                          .WithImageTag(DirectusContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 8055,
                              port: httpPort,
                              name: DirectusResource.HttpEndpointName)
                          .WithEnvironment("ADMIN_EMAIL", adminEmail)
                          .WithEnvironment("DB_CLIENT", "sqlite3")
                          .WithEnvironment("DB_FILENAME", "/directus/database/data.db");

        if (secretParameter != null)
            directus.WithEnvironment("SECRET", secretParameter);

        if (adminPasswordParameter != null)
            directus.WithEnvironment("ADMIN_PASSWORD", adminPasswordParameter);

        directus.WithDataVolume();

        return directus;
    }

    /// <summary>Adds a named volume for the database folder to a Directus container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<DirectusResource> WithDataVolume(this IResourceBuilder<DirectusResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/directus/database",
            isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the database folder to a Directus container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<DirectusResource> WithDataBindMount(this IResourceBuilder<DirectusResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/directus/database", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Directus server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="directusResource">The Directus server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<DirectusResource> directusResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(directusResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{directusResource.Resource.Name}"] = directusResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class DirectusContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "directus/directus";

    internal const string Tag = "latest";
}
