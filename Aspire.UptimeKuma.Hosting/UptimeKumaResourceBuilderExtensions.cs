namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class UptimeKumaResourceBuilderExtensions
{
    /// <summary>
    /// Adds the <see cref="UptimeKumaResource"/> to the given
    /// <paramref name="builder"/> instance.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="httpPort">The HTTP port.</param>
    /// <returns>
    /// An <see cref="IResourceBuilder{UptimeKumaResource}"/> instance that
    /// represents the added Uptime Kuma resource.
    /// </returns>
    public static IResourceBuilder<UptimeKumaResource> AddUptimeKuma(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "uptime-kuma",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new UptimeKumaResource(name);

        return builder.AddResource(resource)
                      .WithImage(UptimeKumaContainerImageTags.Image)
                      .WithImageRegistry(UptimeKumaContainerImageTags.Registry)
                      .WithImageTag(UptimeKumaContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 3001,
                          port: httpPort,
                          name: UptimeKumaResource.HttpEndpointName);
    }

    /// <summary>Adds a named volume for the data folder to an Uptime Kuma container resource.</summary>
    /// <param name="builder">The resource builder.</param>
    /// <param name="name">The name of the volume. Defaults to an auto-generated name based on the resource name.</param>
    /// <param name="isReadOnly">A flag that indicates if this is a read-only volume.</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<UptimeKumaResource> WithDataVolume(this IResourceBuilder<UptimeKumaResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    /// <summary>
    /// Add a reference to an Uptime Kuma server to the resource.
    /// </summary>
    /// <param name="builder">An <see cref="IResourceBuilder{T}"/> for <see cref="ProjectResource"/></param>
    /// <param name="uptimeKumaResource">The Uptime Kuma server resource</param>
    /// <returns>The <see cref="IResourceBuilder{T}"/>.</returns>
    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<UptimeKumaResource> uptimeKumaResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(uptimeKumaResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{uptimeKumaResource.Resource.Name}"] = uptimeKumaResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class UptimeKumaContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "louislam/uptime-kuma";

    internal const string Tag = "1";
}
