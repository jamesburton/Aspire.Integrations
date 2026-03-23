namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class InfisicalResourceBuilderExtensions
{
    public static IResourceBuilder<InfisicalResource> AddInfisical(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "infisical",
        int? httpPort = null,
        ParameterResource? encryptionKeyParameter = null,
        ParameterResource? authSecretParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new InfisicalResource(name, encryptionKeyParameter);

        var infisical = builder.AddResource(resource)
            .WithImage(InfisicalContainerImageTags.Image)
            .WithImageRegistry(InfisicalContainerImageTags.Registry)
            .WithImageTag(InfisicalContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: 8080,
                port: httpPort,
                name: InfisicalResource.HttpEndpointName);

        if (encryptionKeyParameter != null)
            infisical.WithEnvironment("ENCRYPTION_KEY", encryptionKeyParameter);

        if (authSecretParameter != null)
            infisical.WithEnvironment("AUTH_SECRET", authSecretParameter);

        infisical.WithEnvironment(context =>
        {
            context.EnvironmentVariables["SITE_URL"] = resource.HttpEndpoint.Property(EndpointProperty.Url);
        });

        return infisical;
    }

    public static IResourceBuilder<InfisicalResource> WithDataVolume(this IResourceBuilder<InfisicalResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/app/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<InfisicalResource> infisicalResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(infisicalResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{infisicalResource.Resource.Name}"] = infisicalResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class InfisicalContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "infisical/infisical";

    internal const string Tag = "latest-postgres";
}
