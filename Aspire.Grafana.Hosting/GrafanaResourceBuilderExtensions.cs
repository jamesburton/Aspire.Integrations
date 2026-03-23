namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class GrafanaResourceBuilderExtensions
{
    public static IResourceBuilder<GrafanaResource> AddGrafana(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "grafana",
        int? httpPort = null,
        string adminUser = "admin",
        ParameterResource? adminPasswordParameter = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new GrafanaResource(name, adminPasswordParameter);

        var grafana = builder.AddResource(resource)
                          .WithImage(GrafanaContainerImageTags.Image)
                          .WithImageRegistry(GrafanaContainerImageTags.Registry)
                          .WithImageTag(GrafanaContainerImageTags.Tag)
                          .WithHttpEndpoint(
                              targetPort: 3000,
                              port: httpPort,
                              name: GrafanaResource.HttpEndpointName)
                          .WithEnvironment("GF_SECURITY_ADMIN_USER", adminUser)
                          .WithDataVolume();

        if (adminPasswordParameter != null)
            grafana.WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", adminPasswordParameter);

        return grafana;
    }

    public static IResourceBuilder<GrafanaResource> WithDataVolume(this IResourceBuilder<GrafanaResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/var/lib/grafana", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<GrafanaResource> grafanaResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(grafanaResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{grafanaResource.Resource.Name}"] = grafanaResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class GrafanaContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "grafana/grafana-oss";

    internal const string Tag = "latest";
}
