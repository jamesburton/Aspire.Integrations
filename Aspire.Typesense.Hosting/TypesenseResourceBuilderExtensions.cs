namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class TypesenseResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="TypesenseResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="apiKeyParameter">The API key parameter for authentication.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{TypesenseResource}"/> instance that
    /// represents the added Typesense resource.
    /// </returns>
    public static IResourceBuilder<TypesenseResource> AddTypesense(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "typesense",
        int? httpPort = null,
        ParameterResource? apiKeyParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new TypesenseResource(name, apiKeyParameter);

        var typesense = builder.AddResource(resource)
                            .WithImage(TypesenseContainerImageTags.Image)
                            .WithImageRegistry(TypesenseContainerImageTags.Registry)
                            .WithImageTag(TypesenseContainerImageTags.Tag)
                            .WithHttpEndpoint(
                                targetPort: 8108,
                                port: httpPort,
                                name: TypesenseResource.HttpEndpointName)
                            .WithEnvironment("TYPESENSE_DATA_DIR", "/data");

        if (apiKeyParameter != null)
            typesense.WithEnvironment("TYPESENSE_API_KEY", apiKeyParameter);

        return typesense;
    }

    /// <summary>Adds a named volume for the data folder to a Typesense container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TypesenseResource> WithDataVolume(this IResourceBuilder<TypesenseResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/data", isReadOnly);
    }

    /// <summary>
    /// Adds a bind mount for the data folder to a Typesense container resource.
    /// </summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="source">The source directory on the host to mount into the container.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only mount.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TypesenseResource> WithDataBindMount(this IResourceBuilder<TypesenseResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a Typesense server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="typesenseResource">The Typesense server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<TypesenseResource> typesenseResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(typesenseResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{typesenseResource.Resource.Name}"] = typesenseResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class TypesenseContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "typesense/typesense";

    internal const string Tag = "27.1";
}
