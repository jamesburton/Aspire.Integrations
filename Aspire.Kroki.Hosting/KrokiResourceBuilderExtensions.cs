namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class KrokiResourceBuilderExtensions
{
    public static IResourceBuilder<KrokiResource> AddKroki(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "kroki",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new KrokiResource(name);

        return builder.AddResource(resource)
            .WithImage(KrokiContainerImageTags.Image)
            .WithImageRegistry(KrokiContainerImageTags.Registry)
            .WithImageTag(KrokiContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: 8000,
                port: httpPort,
                name: KrokiResource.HttpEndpointName);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<KrokiResource> krokiResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(krokiResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{krokiResource.Resource.Name}"] = krokiResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class KrokiContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "yuzutech/kroki";

    internal const string Tag = "latest";
}
