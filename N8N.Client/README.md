# N8N.Client

A .NET RestSharp-based client library for the [N8N](https://n8n.io/) workflow automation API.

## Features

- **Workflow Management**: Full CRUD operations for workflows with activation/deactivation
- **Execution Monitoring**: List, retrieve, delete, and retry workflow executions
- **Credential Management**: Create, delete, and manage workflow credentials
- **Tag Management**: Organize workflows with tags
- **User Management**: Enterprise user and role management
- **Variable Management**: Environment variables for workflows
- **Project Management**: Multi-tenant project organization
- **Security Auditing**: Generate comprehensive security reports
- **Source Control**: Pull workflows from version control
- **API Key Authentication**: Secure authentication using X-N8N-API-KEY header
- **Type-Safe**: Strongly typed models for all API interactions
- **Async/Await**: Fully asynchronous API with cancellation token support
- **Pagination Support**: Handle large datasets with cursor-based pagination

## Installation

Add the project reference or NuGet package:

```xml
<PackageReference Include="N8N.Client" Version="1.0.0" />
```

## Quick Start

### Basic Setup

```csharp
using N8N.Client;
using N8N.Client.Models;

// Configure the client
var options = new N8NClientOptions
{
    BaseUrl = "http://localhost:5678", // Your N8N instance URL
    ApiKey = "your-api-key-here"       // Your N8N API key
};

using var client = new N8NClient(options);
```

### List Workflows

```csharp
// Get all workflows
var workflows = await client.GetWorkflowsAsync();

// Get workflows with filtering
var activeWorkflows = await client.GetWorkflowsAsync(new ListWorkflowsOptions
{
    Active = true,
    Limit = 50,
    Tags = "production,important"
});

foreach (var workflow in activeWorkflows.Data)
{
    Console.WriteLine($"Workflow: {workflow.Name} (Active: {workflow.Active})");
}
```

### Workflow Management

```csharp
// Get a specific workflow
var workflow = await client.GetWorkflowAsync("workflow-id");

// Create a new workflow
var newWorkflow = new CreateWorkflowRequest
{
    Name = "My New Workflow",
    Nodes = [
        new Node
        {
            Id = "start",
            Name = "Start",
            Type = "n8n-nodes-base.start",
            Position = [100, 200]
        }
    ],
    Connections = [],
    Settings = new WorkflowSettings
    {
        SaveExecutionProgress = false,
        ExecutionTimeout = 3600
    }
};

var created = await client.CreateWorkflowAsync(newWorkflow);

// Activate/Deactivate workflows
await client.ActivateWorkflowAsync(created.Id);
await client.DeactivateWorkflowAsync(created.Id);

// Update a workflow
var updateRequest = new UpdateWorkflowRequest
{
    Name = "Updated Workflow Name",
    Settings = new WorkflowSettings
    {
        ExecutionTimeout = 7200
    }
};
await client.UpdateWorkflowAsync(created.Id, updateRequest);

// Delete a workflow
await client.DeleteWorkflowAsync(created.Id);
```

### Execution Monitoring

```csharp
// List recent executions
var executions = await client.GetExecutionsAsync(new ListExecutionsOptions
{
    Limit = 10,
    Status = ExecutionStatusFilter.Success,
    IncludeData = false
});

foreach (var execution in executions.Data)
{
    Console.WriteLine($"Execution {execution.Id}: {execution.Status}");
}

// Get execution details
var execution = await client.GetExecutionAsync(1234, new GetExecutionOptions
{
    IncludeData = true
});

// Retry a failed execution
await client.RetryExecutionAsync(1234, new RetryExecutionRequest
{
    LoadWorkflow = true // Use latest workflow version
});

// Delete an execution
await client.DeleteExecutionAsync(1234);
```

### Credential Management

```csharp
// Create a credential
var credential = new Credential
{
    Name = "My API Credential",
    Type = "httpBasicAuth",
    Data = new Dictionary<string, object>
    {
        ["user"] = "username",
        ["password"] = "password"
    }
};

var created = await client.CreateCredentialAsync(credential);

// Get credential schema for a type
var schema = await client.GetCredentialTypeSchemaAsync("githubApi");

// Delete a credential
await client.DeleteCredentialAsync("credential-id");
```

### Tag Management

```csharp
// List all tags
var tags = await client.GetTagsAsync();

// Create a tag
var newTag = new Tag { Name = "Production" };
var created = await client.CreateTagAsync(newTag);

// Update a tag
var updated = new Tag { Name = "Production-v2" };
await client.UpdateTagAsync(created.Id, updated);

// Delete a tag
await client.DeleteTagAsync(created.Id);

// Manage workflow tags
await client.UpdateWorkflowTagsAsync("workflow-id", new UpdateWorkflowTagsRequest
{
    TagIds = [new TagIdReference { Id = "tag-id" }]
});
```

## Enterprise Features

### User Management

```csharp
// List users (requires enterprise license)
var users = await client.GetUsersAsync(new ListUsersOptions
{
    IncludeRole = true,
    ProjectId = "project-id"
});

// Create users
var newUsers = new[]
{
    new CreateUserRequest
    {
        Email = "user@company.com",
        Role = UserRoles.GlobalMember
    }
};
await client.CreateUsersAsync(newUsers);

// Change user role
await client.ChangeUserRoleAsync("user-id", new ChangeUserRoleRequest
{
    NewRoleName = UserRoles.GlobalAdmin
});

// Delete a user
await client.DeleteUserAsync("user-id");
```

### Project Management

```csharp
// List projects
var projects = await client.GetProjectsAsync();

// Create a project
var project = new Project { Name = "Marketing Team" };
await client.CreateProjectAsync(project);

// Add users to project
await client.AddUsersToProjectAsync("project-id", new AddUsersToProjectRequest
{
    Relations = [
        new ProjectUserRelation
        {
            UserId = "user-id",
            Role = UserRoles.ProjectEditor
        }
    ]
});

// Change user role in project
await client.ChangeUserRoleInProjectAsync("project-id", "user-id", 
    new ChangeProjectUserRoleRequest
    {
        Role = UserRoles.ProjectAdmin
    });
```

## Variable Management

```csharp
// List variables
var variables = await client.GetVariablesAsync(new ListVariablesOptions
{
    ProjectId = "project-id"
});

// Create a variable
var variable = new VariableCreateRequest
{
    Key = "API_URL",
    Value = "https://api.example.com",
    ProjectId = "project-id"
};
await client.CreateVariableAsync(variable);

// Update a variable
var update = new VariableCreateRequest
{
    Key = "API_URL",
    Value = "https://api-v2.example.com"
};
await client.UpdateVariableAsync("variable-id", update);

// Delete a variable
await client.DeleteVariableAsync("variable-id");
```

## Security Auditing

```csharp
// Generate security audit
var auditRequest = new GenerateAuditRequest
{
    AdditionalOptions = new AuditAdditionalOptions
    {
        DaysAbandonedWorkflow = 30,
        Categories = [
            AuditCategories.Credentials,
            AuditCategories.Database,
            AuditCategories.Instance
        ]
    }
};

var audit = await client.GenerateAuditAsync(auditRequest);

// Process audit results
if (audit.CredentialsRiskReport?.Sections?.Any() == true)
{
    foreach (var section in audit.CredentialsRiskReport.Sections)
    {
        Console.WriteLine($"Issue: {section.Title}");
        Console.WriteLine($"Description: {section.Description}");
        Console.WriteLine($"Recommendation: {section.Recommendation}");
    }
}
```

## Source Control Integration

```csharp
// Pull changes from repository
var pullRequest = new PullRequest
{
    Force = false,
    Variables = new Dictionary<string, object>
    {
        ["ENVIRONMENT"] = "production"
    }
};

var importResult = await client.PullFromRepositoryAsync(pullRequest);

Console.WriteLine($"Imported {importResult.Workflows?.Count ?? 0} workflows");
Console.WriteLine($"Added {importResult.Variables?.Added?.Count ?? 0} variables");
Console.WriteLine($"Changed {importResult.Variables?.Changed?.Count ?? 0} variables");
```

## Configuration Options

```csharp
var options = new N8NClientOptions
{
    BaseUrl = "https://your-n8n-instance.com",
    ApiKey = "your-api-key",
    TimeoutMilliseconds = 60000,  // 60 seconds
    UserAgent = "MyApp/1.0",
    MaxRetryAttempts = 3,
    RetryDelayMilliseconds = 1000,
    ValidateSslCertificates = true
};
```

## Error Handling

```csharp
try
{
    var workflows = await client.GetWorkflowsAsync();
    Console.WriteLine($"Found {workflows.Data.Count()} workflows");
}
catch (N8NClientException ex)
{
    Console.WriteLine($"N8N API error: {ex.Message}");
    if (ex.StatusCode.HasValue)
    {
        Console.WriteLine($"Status Code: {ex.StatusCode}");
        Console.WriteLine($"Response: {ex.ResponseContent}");
    }
    if (!string.IsNullOrEmpty(ex.ErrorCode))
    {
        Console.WriteLine($"Error Code: {ex.ErrorCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Pagination

```csharp
// Handle paginated results
var allWorkflows = new List<Workflow>();
string? cursor = null;

do
{
    var page = await client.GetWorkflowsAsync(new ListWorkflowsOptions
    {
        Limit = 100,
        Cursor = cursor
    });
    
    allWorkflows.AddRange(page.Data);
    cursor = page.NextCursor;
    
} while (!string.IsNullOrEmpty(cursor));

Console.WriteLine($"Total workflows: {allWorkflows.Count}");
```

## API Endpoints Covered

### Workflow Management
- **GET** `/api/v1/workflows` - List workflows
- **GET** `/api/v1/workflows/{id}` - Get workflow
- **POST** `/api/v1/workflows` - Create workflow
- **PUT** `/api/v1/workflows/{id}` - Update workflow
- **DELETE** `/api/v1/workflows/{id}` - Delete workflow
- **POST** `/api/v1/workflows/{id}/activate` - Activate workflow
- **POST** `/api/v1/workflows/{id}/deactivate` - Deactivate workflow
- **PUT** `/api/v1/workflows/{id}/transfer` - Transfer workflow
- **GET** `/api/v1/workflows/{id}/tags` - Get workflow tags
- **PUT** `/api/v1/workflows/{id}/tags` - Update workflow tags

### Execution Management
- **GET** `/api/v1/executions` - List executions
- **GET** `/api/v1/executions/{id}` - Get execution
- **DELETE** `/api/v1/executions/{id}` - Delete execution
- **POST** `/api/v1/executions/{id}/retry` - Retry execution

### Credential Management
- **POST** `/api/v1/credentials` - Create credential
- **DELETE** `/api/v1/credentials/{id}` - Delete credential
- **GET** `/api/v1/credentials/schema/{type}` - Get credential schema
- **PUT** `/api/v1/credentials/{id}/transfer` - Transfer credential

### Tag Management
- **GET** `/api/v1/tags` - List tags
- **GET** `/api/v1/tags/{id}` - Get tag
- **POST** `/api/v1/tags` - Create tag
- **PUT** `/api/v1/tags/{id}` - Update tag
- **DELETE** `/api/v1/tags/{id}` - Delete tag

### User Management (Enterprise)
- **GET** `/api/v1/users` - List users
- **GET** `/api/v1/users/{id}` - Get user
- **POST** `/api/v1/users` - Create users
- **DELETE** `/api/v1/users/{id}` - Delete user
- **PATCH** `/api/v1/users/{id}/role` - Change user role

### Variable Management
- **GET** `/api/v1/variables` - List variables
- **POST** `/api/v1/variables` - Create variable
- **PUT** `/api/v1/variables/{id}` - Update variable
- **DELETE** `/api/v1/variables/{id}` - Delete variable

### Project Management
- **GET** `/api/v1/projects` - List projects
- **POST** `/api/v1/projects` - Create project
- **PUT** `/api/v1/projects/{id}` - Update project
- **DELETE** `/api/v1/projects/{id}` - Delete project
- **POST** `/api/v1/projects/{id}/users` - Add users to project
- **DELETE** `/api/v1/projects/{id}/users/{userId}` - Remove user from project
- **PATCH** `/api/v1/projects/{id}/users/{userId}` - Change user role in project

### Security & Operations
- **POST** `/api/v1/audit` - Generate security audit
- **POST** `/api/v1/source-control/pull` - Pull from repository

## Requirements

- .NET 9.0 or later
- RestSharp 112.0.0 or later
- System.Text.Json 9.0.0 or later
- Valid N8N instance with API access enabled

## Authentication

This client uses the `X-N8N-API-KEY` header for authentication. To obtain an API key:

1. Open your N8N instance
2. Go to Settings → API
3. Generate a new API key
4. Use this key in the client configuration

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## N8N Version Compatibility

This client is designed to work with N8N API version 1.1.1 and later. Some enterprise features require a valid N8N license.
