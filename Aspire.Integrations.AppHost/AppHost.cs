using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var services = builder.Configuration.GetSection("Services");
bool IsEnabled(string name) => services.GetValue<bool>(name);

// --- Core infrastructure ---

var cache = IsEnabled("Redis")
    ? builder.AddRedis("cache")
    : null;

var maildev = IsEnabled("MailDev")
    ? builder.AddMailDev("maildev")
    : null;

IResourceBuilder<QdrantServerResource>? qdrant = null;
if (IsEnabled("Qdrant"))
{
    qdrant = builder.AddQdrant("qdrant")
        .WithLifetime(ContainerLifetime.Persistent)
        .WithDataVolume(name: "qdrant_Aspire-Integrations");
}

// --- AI/LLM tooling ---

IResourceBuilder<FlowiseResource>? flowise = null;
if (IsEnabled("Flowise"))
{
    var flowiseUsernameParameter = builder.AddParameter("flowise-username");
    var flowisePasswordParameter = builder.AddParameter("flowise-password");
    var flowiseApiKeyParameter = builder.AddParameter("flowise-apikey");

    flowise = builder.AddFlowise("flowise",
            apiKeyParameter: flowiseApiKeyParameter.Resource,
            usernameParameter: flowiseUsernameParameter.Resource,
            passwordParameter: flowisePasswordParameter.Resource)
        .WithLifetime(ContainerLifetime.Persistent);
}

var n8n = IsEnabled("N8N")
    ? builder.AddN8N("n8n").WithLifetime(ContainerLifetime.Persistent)
    : null;

var litellm = IsEnabled("LiteLLM")
    ? builder.AddLiteLLM("litellm").WithLifetime(ContainerLifetime.Persistent)
    : null;

var langfuse = IsEnabled("Langfuse")
    ? builder.AddLangfuse("langfuse").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var openWebUI = IsEnabled("OpenWebUI")
    ? builder.AddOpenWebUI("open-webui").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var localai = IsEnabled("LocalAI")
    ? builder.AddLocalAI("localai").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Observability & monitoring ---

var openObserve = IsEnabled("OpenObserve")
    ? builder.AddOpenObserve("openobserve").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var dozzle = IsEnabled("Dozzle")
    ? builder.AddDozzle("dozzle")
    : null;

var uptimeKuma = IsEnabled("UptimeKuma")
    ? builder.AddUptimeKuma("uptime-kuma").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var grafana = IsEnabled("Grafana")
    ? builder.AddGrafana("grafana").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var otelLgtm = IsEnabled("OtelLgtm")
    ? builder.AddOtelLgtm("otel-lgtm").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Search & storage ---

var meilisearch = IsEnabled("Meilisearch")
    ? builder.AddMeilisearch("meilisearch").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var typesense = IsEnabled("Typesense")
    ? builder.AddTypesense("typesense").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var seaweedfs = IsEnabled("SeaweedFS")
    ? builder.AddSeaweedFS("seaweedfs").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var memcached = IsEnabled("Memcached")
    ? builder.AddMemcached("memcached")
    : null;

// --- Developer utilities ---

var gotenberg = IsEnabled("Gotenberg")
    ? builder.AddGotenberg("gotenberg")
    : null;

var kroki = IsEnabled("Kroki")
    ? builder.AddKroki("kroki")
    : null;

var hoppscotch = IsEnabled("Hoppscotch")
    ? builder.AddHoppscotch("hoppscotch").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var adminer = IsEnabled("Adminer")
    ? builder.AddAdminer("adminer")
    : null;

// --- Security & identity ---

var infisical = IsEnabled("Infisical")
    ? builder.AddInfisical("infisical").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var zitadel = IsEnabled("Zitadel")
    ? builder.AddZitadel("zitadel").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Workflow orchestration ---

var temporal = IsEnabled("Temporal")
    ? builder.AddTemporal("temporal").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var windmill = IsEnabled("Windmill")
    ? builder.AddWindmill("windmill").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Knowledge & project management ---

var docmost = IsEnabled("Docmost")
    ? builder.AddDocmost("docmost").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var gitea = IsEnabled("Gitea")
    ? builder.AddGitea("gitea").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var plane = IsEnabled("Plane")
    ? builder.AddPlane("plane").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Content & low-code ---

var directus = IsEnabled("Directus")
    ? builder.AddDirectus("directus").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var appsmith = IsEnabled("Appsmith")
    ? builder.AddAppsmith("appsmith").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Database admin & migrations ---

var pgadmin = IsEnabled("PgAdmin")
    ? builder.AddPgAdmin("pgadmin").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var bytebase = IsEnabled("Bytebase")
    ? builder.AddBytebase("bytebase").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

// --- Messaging ---

var mattermost = IsEnabled("Mattermost")
    ? builder.AddMattermost("mattermost").WithLifetime(ContainerLifetime.Persistent).WithDataVolume()
    : null;

var soketi = IsEnabled("Soketi")
    ? builder.AddSoketi("soketi")
    : null;

// --- Application projects ---

var apiService = builder.AddProject<Projects.Aspire_Integrations_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

if (qdrant is not null) apiService.WithReference(qdrant).WaitFor(qdrant);
if (maildev is not null) apiService.WithReference(maildev).WaitFor(maildev);
if (flowise is not null) apiService.WithReference(flowise).WaitFor(flowise);
if (n8n is not null) apiService.WithReference(n8n).WaitFor(n8n);
if (gotenberg is not null) apiService.WithReference(gotenberg).WaitFor(gotenberg);
if (meilisearch is not null) apiService.WithReference(meilisearch).WaitFor(meilisearch);

var webfrontend = builder.AddProject<Projects.Aspire_Integrations_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

if (cache is not null) webfrontend.WithReference(cache).WaitFor(cache);

await builder.Build().RunAsync();
