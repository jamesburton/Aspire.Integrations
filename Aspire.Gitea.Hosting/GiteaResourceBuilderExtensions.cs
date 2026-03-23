namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class GiteaResourceBuilderExtensions
{
    public static IResourceBuilder<GiteaResource> AddGitea(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "gitea",
        int? httpPort = null,
        int? sshPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new GiteaResource(name);

        var gitea = builder.AddResource(resource)
            .WithImage(GiteaContainerImageTags.Image)
            .WithImageRegistry(GiteaContainerImageTags.Registry)
            .WithImageTag(GiteaContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: 3000,
                port: httpPort,
                name: GiteaResource.HttpEndpointName)
            .WithEndpoint(
                targetPort: 22,
                port: sshPort,
                name: GiteaResource.SshEndpointName)
            .WithEnvironment("GITEA__server__DOMAIN", "localhost")
            .WithEnvironment(context =>
            {
                var httpEndpoint = resource.HttpEndpoint;
                context.EnvironmentVariables["GITEA__server__ROOT_URL"] = httpEndpoint.Property(EndpointProperty.Url);
            })
            .WithDataVolume();

        return gitea;
    }

    public static IResourceBuilder<GiteaResource> WithDataVolume(this IResourceBuilder<GiteaResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<GiteaResource> giteaResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(giteaResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{giteaResource.Resource.Name}"] = giteaResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class GiteaContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "gitea/gitea";

    internal const string Tag = "latest";
}
