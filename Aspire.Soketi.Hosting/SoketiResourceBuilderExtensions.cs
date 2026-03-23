namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class SoketiResourceBuilderExtensions
{
    public static IResourceBuilder<SoketiResource> AddSoketi(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "soketi",
        int? wsPort = null,
        string appId = "app-id",
        string appKey = "app-key",
        string appSecret = "app-secret")
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new SoketiResource(name);

        return builder.AddResource(resource)
            .WithImage(SoketiContainerImageTags.Image)
            .WithImageRegistry(SoketiContainerImageTags.Registry)
            .WithImageTag(SoketiContainerImageTags.Tag)
            .WithEndpoint(
                targetPort: 6001,
                port: wsPort,
                name: SoketiResource.WsEndpointName,
                scheme: "ws")
            .WithEnvironment("SOKETI_DEFAULT_APP_ID", appId)
            .WithEnvironment("SOKETI_DEFAULT_APP_KEY", appKey)
            .WithEnvironment("SOKETI_DEFAULT_APP_SECRET", appSecret)
            .WithEnvironment("SOKETI_METRICS_ENABLED", "true");
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(
        this IResourceBuilder<TDestination> builder,
        IResourceBuilder<SoketiResource> soketiResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(soketiResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{soketiResource.Resource.Name}"] =
                soketiResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class SoketiContainerImageTags
{
    internal const string Registry = "quay.io";

    internal const string Image = "soketi/soketi";

    internal const string Tag = "1.6-16-debian";
}
