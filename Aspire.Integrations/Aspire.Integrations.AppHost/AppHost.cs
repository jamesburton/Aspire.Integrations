var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var maildev = builder.AddMailDev("maildev");

var qdrant = builder.AddQdrant("qdrant")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithDataVolume(name: "qdrant_Aspire-Integrations");

var apiService = builder.AddProject<Projects.Aspire_Integrations_ApiService>("apiservice")
    .WithReference(qdrant)
    .WaitFor(qdrant)
    .WithReference(maildev)
    .WaitFor(maildev)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Aspire_Integrations_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();