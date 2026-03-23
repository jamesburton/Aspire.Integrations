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

// API development & security
var hoppscotch = builder.AddHoppscotch("hoppscotch")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var infisical = builder.AddInfisical("infisical")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var zitadel = builder.AddZitadel("zitadel")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Workflow orchestration
var temporal = builder.AddTemporal("temporal")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var windmill = builder.AddWindmill("windmill")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Knowledge & project management
var docmost = builder.AddDocmost("docmost")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var gitea = builder.AddGitea("gitea")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var plane = builder.AddPlane("plane")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Visualization & low-code
var kroki = builder.AddKroki("kroki");

var appsmith = builder.AddAppsmith("appsmith")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Database admin & migrations
var adminer = builder.AddAdminer("adminer");

var pgadmin = builder.AddPgAdmin("pgadmin")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var bytebase = builder.AddBytebase("bytebase")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Observability & dashboards
var grafana = builder.AddGrafana("grafana")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var otelLgtm = builder.AddOtelLgtm("otel-lgtm")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

// Caching & messaging
var memcached = builder.AddMemcached("memcached");

var mattermost = builder.AddMattermost("mattermost")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var soketi = builder.AddSoketi("soketi");

// Content & AI
var directus = builder.AddDirectus("directus")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var localai = builder.AddLocalAI("localai")
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
