namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class ZitadelResourceBuilderExtensions
{
    public static IResourceBuilder<ZitadelResource> AddZitadel(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "zitadel",
        int? httpPort = null,
        ParameterResource? masterKeyParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new ZitadelResource(name, masterKeyParameter);

        var zitadel = builder.AddResource(resource)
                          .WithImage(ZitadelContainerImageTags.Image)
                          .WithImageRegistry(ZitadelContainerImageTags.Registry)
                          .WithImageTag(ZitadelContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 8080,
                              port: httpPort,
                              name: ZitadelResource.HttpEndpointName)
                          .WithArgs("start-from-init", "--masterkeyFromEnv", "--tlsMode", "disabled")
                          .WithEnvironment("ZITADEL_EXTERNALDOMAIN", "localhost")
                          .WithEnvironment("ZITADEL_EXTERNALPORT", "8080");

        if (masterKeyParameter != null)
            zitadel.WithEnvironment("ZITADEL_MASTERKEY", masterKeyParameter);

        return zitadel;
    }

    public static IResourceBuilder<ZitadelResource> WithDataVolume(this IResourceBuilder<ZitadelResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/data", isReadOnly);
    }

    public static IResourceBuilder<ZitadelResource> WithDataBindMount(this IResourceBuilder<ZitadelResource> builder, string source, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(source);

        return builder.WithBindMount(source, "/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<ZitadelResource> zitadelResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(zitadelResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{zitadelResource.Resource.Name}"] = zitadelResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class ZitadelContainerImageTags
{
    internal const string Registry = "ghcr.io";

    internal const string Image = "zitadel/zitadel";

    internal const string Tag = "latest";
}
