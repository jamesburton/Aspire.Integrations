# N8N Aspire Integration

A comprehensive .NET Aspire integration for [N8N](https://n8n.io/) workflow automation, providing both hosting and client capabilities.

## Components

This integration consists of three main packages:

1. **N8N.Client** - Core RestSharp-based client library for N8N API
2. **Aspire.N8N.Hosting** - Aspire hosting integration for running N8N containers
3. **Aspire.N8N.Client** - Aspire client integration with dependency injection and health checks

## Features

### Hosting Integration (Aspire.N8N.Hosting)
- **Container Management**: Deploy N8N as containerized resource
- **Data Persistence**: Volume and bind mount support for workflow data
- **Environment Configuration**: API key, host, port, and webhook URL setup
- **Connection String Generation**: Automatic connection string creation for client apps

### Client Integration (Aspire.N8N.Client)
- **Dependency Injection**: Seamless integration with .NET DI container
- **Health Checks**: Built-in health monitoring for N8N connectivity
- **Configuration**: Flexible configuration via connection strings or settings
- **Scoped Clients**: Proper lifetime management for N8N client instances

### Core Client (N8N.Client)
- **Full API Coverage**: Complete N8N REST API implementation
- **Workflow Management**: CRUD operations, activation/deactivation
- **Execution Monitoring**: Track, retry, and manage workflow executions
- **Credential Management**: Secure credential storage and retrieval
- **Enterprise Features**: User management, projects, variables, auditing
- **Type Safety**: Strongly typed models for all operations
- **Async/Await**: Non-blocking operations with cancellation support

## Quick Start

### 1. Add N8N to Your AppHost

```csharp
// Program.cs in your AppHost project
var builder = DistributedApplication.CreateBuilder(args);

// Option 1: Basic N8N setup
var n8n = builder.AddN8N("n8n")
                 .WithDataVolume();

// Option 2: N8N with API key
var apiKey = builder.AddParameter("n8n-api-key", secret: true);
var n8n = builder.AddN8N("n8n", apiKeyParameter: apiKey)
                 .WithDataVolume()
                 .WithEnvironment("N8N_BASIC_AUTH_ACTIVE", "true")
                 .WithEnvironment("N8N_BASIC_AUTH_USER", "admin")
                 .WithEnvironment("N8N_BASIC_AUTH_PASSWORD", "admin");

// Add your application with N8N reference
builder.AddProject<Projects.MyApp>("myapp")
       .WithReference(n8n);

builder.Build().Run();
```

### 2. Configure Client in Your Application

```csharp
// Program.cs in your application
var builder = WebApplication.CreateBuilder(args);

// Add N8N client using connection string from hosting
builder.AddN8NClient("n8n");

// Alternative: Configure with settings
builder.AddN8NClient("n8n", settings =>
{
    settings.Endpoint = new Uri("http://localhost:5678");
    settings.ApiKey = "your-api-key";
    settings.DisableHealthChecks = false;
});

var app = builder.Build();
app.Run();
```

### 3. Use N8N Client in Your Services

```csharp
[ApiController]
[Route("api/[controller]")]
public class WorkflowController(IN8NClient n8nClient) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetWorkflows()
    {
        var workflows = await n8nClient.GetWorkflowsAsync(new ListWorkflowsOptions
        {
            Active = true,
            Limit = 50
        });
        
        return Ok(workflows.Data);
    }

    [HttpPost("{workflowId}/activate")]
    public async Task<IActionResult> ActivateWorkflow(string workflowId)
    {
        var workflow = await n8nClient.ActivateWorkflowAsync(workflowId);
        return Ok(new { workflow.Name, workflow.Active });
    }

    [HttpGet("executions")]
    public async Task<IActionResult> GetExecutions()
    {
        var executions = await n8nClient.GetExecutionsAsync(new ListExecutionsOptions
        {
            Limit = 10,
            Status = ExecutionStatusFilter.Success
        });
        
        return Ok(executions.Data);
    }
}
```

## Configuration

### Connection String Format

```bash
# Format 1: Simple URL (for local development)
"http://localhost:5678"

# Format 2: With API Key
"Endpoint=http://localhost:5678;Key=your-api-key-here"
```

### Configuration Sections

```json
{
  "ConnectionStrings": {
    "n8n": "Endpoint=http://localhost:5678;Key=your-api-key"
  },
  "N8N": {
    "Client": {
      "Endpoint": "http://localhost:5678",
      "ApiKey": "your-api-key",
      "DisableHealthChecks": false,
      "DisableTracing": false,
      "DisableMetrics": false
    }
  }
}
```

## Advanced Hosting Scenarios

### N8N with Custom Configuration

```csharp
var n8n = builder.AddN8N("n8n", httpPort: 8080)
                 .WithEnvironment("N8N_ENCRYPTION_KEY", "your-encryption-key")
                 .WithEnvironment("N8N_USER_MANAGEMENT_DISABLED", "false")
                 .WithEnvironment("EXECUTIONS_TIMEOUT", "300")
                 .WithEnvironment("EXECUTIONS_TIMEOUT_MAX", "3600")
                 .WithDataVolume();
```

### N8N with Persistent Storage

```csharp
// Using named volume
var n8n = builder.AddN8N("n8n")
                 .WithDataVolume("n8n-data");

// Using bind mount
var n8n = builder.AddN8N("n8n")
                 .WithDataBindMount("./n8n-data");
```

### Multiple N8N Instances

```csharp
// Development instance
var n8nDev = builder.AddN8N("n8n-dev", httpPort: 5678)
                    .WithDataVolume("n8n-dev-data");

// Staging instance
var n8nStaging = builder.AddN8N("n8n-staging", httpPort: 5679)
                        .WithDataVolume("n8n-staging-data");

// Reference different instances
builder.AddProject<Projects.DevApp>("devapp")
       .WithReference(n8nDev);

builder.AddProject<Projects.StagingApp>("stagingapp")
       .WithReference(n8nStaging);
```

## Client Usage Examples

### Workflow Management

```csharp
public class WorkflowService(IN8NClient client)
{
    public async Task<string> CreateSimpleWorkflow()
    {
        var workflow = new CreateWorkflowRequest
        {
            Name = $"API Created Workflow {DateTime.Now:yyyy-MM-dd}",
            Nodes = [
                new Node
                {
                    Id = "webhook-trigger",
                    Name = "Webhook",
                    Type = "n8n-nodes-base.webhook",
                    Position = [240, 300],
                    Parameters = new Dictionary<string, object>
                    {
                        ["httpMethod"] = "POST",
                        ["path"] = "test-webhook"
                    }
                },
                new Node
                {
                    Id = "set-data",
                    Name = "Set Data",
                    Type = "n8n-nodes-base.set",
                    Position = [460, 300],
                    Parameters = new Dictionary<string, object>
                    {
                        ["values"] = new Dictionary<string, object>
                        {
                            ["message"] = "Hello from N8N!"
                        }
                    }
                }
            ],
            Connections = new Dictionary<string, object>
            {
                ["webhook-trigger"] = new Dictionary<string, object>
                {
                    ["main"] = new object[][]
                    {
                        [new { node = "set-data", type = "main", index = 0 }]
                    }
                }
            },
            Settings = new WorkflowSettings
            {
                ExecutionTimeout = 3600,
                SaveExecutionProgress = false
            }
        };

        var created = await client.CreateWorkflowAsync(workflow);
        await client.ActivateWorkflowAsync(created.Id!);
        
        return created.Id!;
    }

    public async Task<WorkflowListResponse> GetActiveWorkflows()
    {
        return await client.GetWorkflowsAsync(new ListWorkflowsOptions
        {
            Active = true,
            Limit = 100
        });
    }
}
```

### Execution Monitoring

```csharp
public class ExecutionMonitoringService(IN8NClient client)
{
    public async Task<ExecutionStats> GetExecutionStats()
    {
        var executions = await client.GetExecutionsAsync(new ListExecutionsOptions
        {
            Limit = 250,
            IncludeData = false
        });

        var stats = new ExecutionStats();
        foreach (var execution in executions.Data)
        {
            switch (execution.Status)
            {
                case ExecutionStatus.Success:
                    stats.Successful++;
                    break;
                case ExecutionStatus.Error:
                    stats.Failed++;
                    break;
                case ExecutionStatus.Running:
                    stats.Running++;
                    break;
                case ExecutionStatus.Waiting:
                    stats.Waiting++;
                    break;
            }
        }

        return stats;
    }

    public async Task RetryFailedExecutions()
    {
        var failedExecutions = await client.GetExecutionsAsync(new ListExecutionsOptions
        {
            Status = ExecutionStatusFilter.Error,
            Limit = 50
        });

        foreach (var execution in failedExecutions.Data)
        {
            try
            {
                await client.RetryExecutionAsync(execution.Id, new RetryExecutionRequest
                {
                    LoadWorkflow = true
                });
            }
            catch (Exception ex)
            {
                // Log retry failure
                Console.WriteLine($"Failed to retry execution {execution.Id}: {ex.Message}");
            }
        }
    }
}

public class ExecutionStats
{
    public int Successful { get; set; }
    public int Failed { get; set; }
    public int Running { get; set; }
    public int Waiting { get; set; }
}
```

### Enterprise Features

```csharp
public class EnterpriseService(IN8NClient client)
{
    public async Task<string> CreateProjectWorkflow(string projectName)
    {
        // Create project
        var project = new Project { Name = projectName };
        await client.CreateProjectAsync(project);

        // Create project-scoped variable
        var variable = new VariableCreateRequest
        {
            Key = "PROJECT_API_URL",
            Value = $"https://api.{projectName.ToLower()}.com",
            ProjectId = project.Id
        };
        await client.CreateVariableAsync(variable);

        return project.Id!;
    }

    public async Task<AuditResponse> RunSecurityAudit()
    {
        return await client.GenerateAuditAsync(new GenerateAuditRequest
        {
            AdditionalOptions = new AuditAdditionalOptions
            {
                DaysAbandonedWorkflow = 30,
                Categories = [
                    AuditCategories.Credentials,
                    AuditCategories.Instance,
                    AuditCategories.Nodes
                ]
            }
        });
    }
}
```

## Health Checks

The integration automatically registers health checks that verify:

- N8N server connectivity
- API authentication
- Basic API functionality

Access health status at `/health` or configure custom health check endpoints:

```csharp
app.MapHealthChecks("/health/n8n", new HealthCheckOptions
{
    Predicate = check => check.Name.StartsWith("N8N")
});
```

## Error Handling

```csharp
public class SafeWorkflowService(IN8NClient client, ILogger<SafeWorkflowService> logger)
{
    public async Task<bool> TryActivateWorkflow(string workflowId)
    {
        try
        {
            await client.ActivateWorkflowAsync(workflowId);
            return true;
        }
        catch (N8NClientException ex) when (ex.StatusCode == 404)
        {
            logger.LogWarning("Workflow {WorkflowId} not found", workflowId);
            return false;
        }
        catch (N8NClientException ex)
        {
            logger.LogError(ex, "N8N API error activating workflow {WorkflowId}: {Message}", 
                workflowId, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error activating workflow {WorkflowId}", workflowId);
            return false;
        }
    }
}
```

## Testing

### Integration Tests

```csharp
public class N8NIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public N8NIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthCheck_Should_Return_Healthy()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", content);
    }

    [Fact]
    public async Task N8NClient_Should_Connect_Successfully()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var n8nClient = scope.ServiceProvider.GetRequiredService<IN8NClient>();

        // Act & Assert
        var workflows = await n8nClient.GetWorkflowsAsync();
        Assert.NotNull(workflows);
    }
}
```

## Requirements

- .NET 9.0 or later
- Docker (for container hosting)
- N8N instance (local or remote)
- Valid N8N API key (for authenticated access)

## Container Configuration

The N8N container is configured with:

- **Image**: `n8nio/n8n:latest`
- **Port**: 5678 (default)
- **Data Directory**: `/home/node/.n8n`
- **Environment Variables**:
  - `N8N_HOST=0.0.0.0`
  - `N8N_PORT=5678`
  - `WEBHOOK_URL=http://localhost:5678/`

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## N8N Documentation

For more information about N8N:
- [N8N Official Documentation](https://docs.n8n.io/)
- [N8N API Documentation](https://docs.n8n.io/api/)
- [N8N Community](https://community.n8n.io/)
