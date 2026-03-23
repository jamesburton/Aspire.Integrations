namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class MemcachedResourceBuilderExtensions
{
    public static IResourceBuilder<MemcachedResource> AddMemcached(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "memcached",
        int? port = null,
        int memoryLimitMb = 256)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new MemcachedResource(name);

        return builder.AddResource(resource)
                      .WithImage(MemcachedContainerImageTags.Image)
                      .WithImageRegistry(MemcachedContainerImageTags.Registry)
                      .WithImageTag(MemcachedContainerImageTags.Tag)
                      .WithEndpoint(
                          targetPort: 11211,
                          port: port,
                          name: MemcachedResource.MemcachedEndpointName)
                      .WithArgs("memcached", "-m", memoryLimitMb.ToString());
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<MemcachedResource> memcachedResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(memcachedResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{memcachedResource.Resource.Name}"] = memcachedResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class MemcachedContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "memcached";

    internal const string Tag = "1.6-alpine";
}
