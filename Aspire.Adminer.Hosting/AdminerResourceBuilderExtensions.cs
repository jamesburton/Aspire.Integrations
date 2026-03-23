namespace Aspire.Hosting;

using Aspire.Hosting.ApplicationModel;

public static class AdminerResourceBuilderExtensions
{
    public static IResourceBuilder<AdminerResource> AddAdminer(
        this IDistributedApplicationBuilder builder,
        [ResourceName] string name = "adminer",
        int? httpPort = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentException.ThrowIfNullOrEmpty(name);

        var resource = new AdminerResource(name);

        return builder.AddResource(resource)
                      .WithImage(AdminerContainerImageTags.Image)
                      .WithImageRegistry(AdminerContainerImageTags.Registry)
                      .WithImageTag(AdminerContainerImageTags.Tag)
                      .WithHttpEndpoint(
                          targetPort: 8080,
                          port: httpPort,
                          name: AdminerResource.HttpEndpointName)
                      .WithEnvironment("ADMINER_DESIGN", "pepa-linha");
    }

    public static IResourceBuilder<TDestination> WithReference<TDestination>(this IResourceBuilder<TDestination> builder, IResourceBuilder<AdminerResource> adminerResource)
         where TDestination : IResourceWithEnvironment
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(adminerResource);

        builder.WithEnvironment(context =>
        {
            context.EnvironmentVariables[$"ConnectionStrings__{adminerResource.Resource.Name}"] = adminerResource.Resource.ConnectionStringExpression;
        });

        return builder;
    }
}

internal static class AdminerContainerImageTags
{
    internal const string Registry = "docker.io";

    internal const string Image = "adminer";

    internal const string Tag = "5";
}
