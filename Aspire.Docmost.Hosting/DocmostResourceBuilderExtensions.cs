namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class DocmostResourceBuilderExtensions
{
    public static IResourceBuilder<DocmostResource> AddDocmost(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "docmost",
        int? httpPort = null,
        ParameterResource? appSecretParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new DocmostResource(name, appSecretParameter);

        var docmost = builder.AddResource(resource)
                          .WithImage(DocmostContainerImageTags.Image)
                          .WithImageRegistry(DocmostContainerImageTags.Registry)
                          .WithImageTag(DocmostContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 3000,
                              port: httpPort,
                              name: DocmostResource.HttpEndpointName)
                          .WithEnvironment(context =>
                          {
                              context.EnvironmentVariables["APP_URL"] = resource.HttpEndpoint.Property(EndpointProperty.Url);
                          });

        if (appSecretParameter != null)
            docmost.WithEnvironment("APP_SECRET", appSecretParameter);

        return docmost;
    }

    public static IResourceBuilder<DocmostResource> WithDataVolume(this IResourceBuilder<DocmostResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data/storage", isReadOnly);
    }

    public static IResourceBuilder<DocmostResource> WithDataBindMount(this IResourceBuilder<DocmostResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/app/data/storage", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<DocmostResource> docmostResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(docmostResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{docmostResource.Resource.Name}"] = docmostResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class DocmostContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "docmost/docmost";

    internal const string Tag = "latest";
}
