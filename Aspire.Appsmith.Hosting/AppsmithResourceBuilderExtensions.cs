namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class AppsmithResourceBuilderExtensions
{
    public static IResourceBuilder<AppsmithResource> AddAppsmith(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "appsmith",
        int? httpPort = null,
        ParameterResource? encryptionPasswordParameter = null,
        ParameterResource? encryptionSaltParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new AppsmithResource(name, encryptionPasswordParameter);

        var appsmith = builder.AddResource(resource)
                          .WithImage(AppsmithContainerImageTags.Image)
                          .WithImageRegistry(AppsmithContainerImageTags.Registry)
                          .WithImageTag(AppsmithContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 80,
                              port: httpPort,
                              name: AppsmithResource.HttpEndpointName);

        if (encryptionPasswordParameter != null)
            appsmith.WithEnvironment("APPSMITH_ENCRYPTION_PASSWORD", encryptionPasswordParameter);

        if (encryptionSaltParameter != null)
            appsmith.WithEnvironment("APPSMITH_ENCRYPTION_SALT", encryptionSaltParameter);

        return appsmith;
    }

    public static IResourceBuilder<AppsmithResource> WithDataVolume(this IResourceBuilder<AppsmithResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/appsmith-stacks", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<AppsmithResource> appsmithResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(appsmithResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{appsmithResource.Resource.Name}"] = appsmithResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class AppsmithContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "appsmith/appsmith-ce";

    internal const string Tag = "latest";
}
