namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class PgAdminResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="PgAdminResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <param name="adminEmail">The default admin email address.</param>
    /// <param name="passwordParameter">The password parameter for the default admin account.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{PgAdminResource}"/> instance that
    /// represents the added pgAdmin resource.
    /// </returns>
    public static IResourceBuilder<PgAdminResource> AddPgAdmin(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "pgadmin",
        int? httpPort = null,
        string adminEmail = "admin@example.com",
        ParameterResource? passwordParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new PgAdminResource(name, passwordParameter);

        var pgadmin = builder.AddResource(resource)
                         .WithImage(PgAdminContainerImageTags.Image)
                         .WithImageRegistry(PgAdminContainerImageTags.Registry)
                         .WithImageTag(PgAdminContainerImageTags.Tag)
                         .WithHttpEndpoint(
                             targetPort: 80,
                             port: httpPort,
                             name: PgAdminResource.HttpEndpointName)
                         .WithEnvironment("PGADMIN_DEFAULT_EMAIL", adminEmail)
                         .WithDataVolume();

        if (passwordParameter != null)
            pgadmin.WithEnvironment("PGADMIN_DEFAULT_PASSWORD", passwordParameter);

        return pgadmin;
    }

    /// <summary>Adds a named volume for the data folder to a pgAdmin container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<PgAdminResource> WithDataVolume(this IResourceBuilder<PgAdminResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/var/lib/pgadmin", isReadOnly);
    }

    /// <summary>
    /// Add a reference to a pgAdmin server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="pgAdminResource">The pgAdmin server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<PgAdminResource> pgAdminResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(pgAdminResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{pgAdminResource.Resource.Name}"] = pgAdminResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class PgAdminContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "dpage/pgadmin4";

    internal const string Tag = "latest";
}
