var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

//var flowiseSecretKeyParameter = builder.AddParameter("flowise-secret-key");
var flowiseUsernameParameter = builder.AddParameter("flowise-username");
var flowisePasswordParameter = builder.AddParameter("flowise-password");
var flowiseApiKeyParameter = builder.AddParameter("flowise-apikey");

var maildev = builder.AddMailDev("maildev");

var flowise = builder.AddFlowise("flowise",
        apiKeyParameter: flowiseApiKeyParameter.Resource,
        usernameParameter: flowiseUsernameParameter.Resource,
        passwordParameter: flowisePasswordParameter.Resource)
    .WithLifetime(ContainerLifetime.Persistent);

var n8n = builder.AddN8N("n8n")
    .WithLifetime(ContainerLifetime.Persistent);

var qdrant = builder.AddQdrant("qdrant")
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithDataVolume(name: "flowise_Aspire-Integrations");

var apiService = builder.AddProject<Projects.Aspire_Integrations_ApiService>("apiservice")
    .WithReference(qdrant)
    .WaitFor(qdrant)
    .WithReference(maildev)
    .WaitFor(maildev)
    .WithReference(flowise)
    .WaitFor(flowise)
    .WithReference(n8n)
    .WaitFor(n8n)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Aspire_Integrations_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();