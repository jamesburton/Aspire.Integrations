namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class MattermostResourceBuilderExtensions
{
    public static IResourceBuilder<MattermostResource> AddMattermost(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "mattermost",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new MattermostResource(name);

        return builder.AddResource(resource)
            .WithImage(MattermostContainerImageTags.Image)
            .WithImageRegistry(MattermostContainerImageTags.Registry)
            .WithImageTag(MattermostContainerImageTags.Tag)
            .WithHttpEndpoint(
                targetPort: 8065,
                port: httpPort,
                name: MattermostResource.HttpEndpointName)
            .WithEnvironment("MM_SERVICESETTINGS_ENABLELOCALMODE", "true")
            .WithDataVolume();
    }

    public static IResourceBuilder<MattermostResource> WithDataVolume(this IResourceBuilder<MattermostResource> builder, string? name = null, bool isReadOnly = false)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithVolume(name ?? VolumeNameGenerator.Generate(builder, "data"), "/mattermost/data", isReadOnly);
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<MattermostResource> mattermostResource)
        where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(mattermostResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{mattermostResource.Resource.Name}"] = mattermostResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class MattermostContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "mattermost/mattermost-team-edition";

    internal const string Tag = "latest";
}
