namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class LocalAIResourceBuilderExtensions
{
    public static IResourceBuilder<LocalAIResource> AddLocalAI(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "localai",
        int? httpPort = null,
        ParameterResource? apiKeyParameter = null,
        int threads = 4)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new LocalAIResource(name, apiKeyParameter);

        var localai = builder.AddResource(resource)
                             .WithImage(LocalAIContainerImageTags.Image)
                             .WithImageRegistry(LocalAIContainerImageTags.Registry)
                             .WithImageTag(LocalAIContainerImageTags.Tag)
                             .WithHttpEndpoint(
                                 targetPort: 8080,
                                 port: httpPort,
                                 name: LocalAIResource.HttpEndpointName)
                             .WithEnvironment("THREADS", threads.ToString())
                             .WithEnvironment("CONTEXT_SIZE", "4096");

        if (apiKeyParameter != null)
            localai.WithEnvironment("API_KEY", apiKeyParameter);

        localai.WithDataVolume();

        return localai;
    }

    public static IResourceBuilder<LocalAIResource> WithDataVolume(this IResourceBuilder<LocalAIResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "models"), "/models", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<LocalAIResource> localAIResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(localAIResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{localAIResource.Resource.Name}"] = localAIResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class LocalAIContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "localai/localai";

    internal const string Tag = "latest-aio-cpu";
}
