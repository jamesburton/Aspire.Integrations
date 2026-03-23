var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

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
                    .WithDataVolume(name: "qdrant_Aspire-Integrations");

// AI/LLM tooling
var litellm = builder.AddLiteLLM("litellm")
    .WithLifetime(ContainerLifetime.Persistent);

var langfuse = builder.AddLangfuse("langfuse")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var openWebUI = builder.AddOpenWebUI("open-webui")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Observability & monitoring
var openObserve = builder.AddOpenObserve("openobserve")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var dozzle = builder.AddDozzle("dozzle");

var uptimeKuma = builder.AddUptimeKuma("uptime-kuma")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Developer utilities
var gotenberg = builder.AddGotenberg("gotenberg");

var meilisearch = builder.AddMeilisearch("meilisearch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var typesense = builder.AddTypesense("typesense")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var seaweedfs = builder.AddSeaweedFS("seaweedfs")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Application projects
var apiService = builder.AddProject<Projects.Aspire_Integrations_ApiService>("apiservice")
    .WithReference(qdrant)
    .WaitFor(qdrant)
    .WithReference(maildev)
    .WaitFor(maildev)
    .WithReference(flowise)
    .WaitFor(flowise)
    .WithReference(n8n)
    .WaitFor(n8n)
    .WithReference(gotenberg)
    .WaitFor(gotenberg)
    .WithReference(meilisearch)
    .WaitFor(meilisearch)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Aspire_Integrations_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

await builder.Build().RunAsync();
