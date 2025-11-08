# Flowise.Client

A .NET RestSharp-based client library for the [Flowise AI](https://flowiseai.com/) API.

## Features

- **Prediction API**: Send messages to chatflows and receive AI responses
- **Streaming Support**: Real-time streaming responses for better UX
- **Chatflow Management**: Create, read, update, and delete chatflows
- **Session Management**: Maintain conversation context across multiple requests
- **File Upload Support**: Send files along with your messages
- **Comprehensive Configuration**: Override chatflow settings at runtime
- **Type-Safe**: Strongly typed models for all API interactions
- **Async/Await**: Fully asynchronous API with cancellation token support

## Installation

Add the project reference or NuGet package:

```xml
<PackageReference Include="Flowise.Client" Version="1.0.0" />
```

## Quick Start

### Basic Setup

```csharp
using Flowise.Client;
using Flowise.Client.Models;

// Configure the client
var options = new FlowiseClientOptions
{
    BaseUrl = "http://localhost:3000", // Your Flowise instance URL
    ApiKey = "your-api-key-here"       // Optional: API key for protected chatflows
};

using var client = new FlowiseClient(options);
```

### Simple Chat

```csharp
// Create a prediction request
var request = new PredictionRequest
{
    ChatflowId = "your-chatflow-id",
    Question = "Hello! How are you today?"
};

// Send the message and get response
var response = await client.CreatePredictionAsync(request);
Console.WriteLine($"AI Response: {response.Text}");
```

### Streaming Chat

```csharp
var request = new PredictionRequest
{
    ChatflowId = "your-chatflow-id",
    Question = "Tell me a long story about AI",
    Streaming = true
};

// Process streaming response
await foreach (var chunk in client.CreateStreamingPredictionAsync(request))
{
    Console.Write(chunk);
}
```

### Session-Based Conversation

```csharp
// Create override config with session ID
var overrideConfig = OverrideConfigBuilder.Create(
    sessionId: "user-session-123",
    temperature: 0.7,
    maxTokens: 500,
    returnSourceDocuments: true
);

var request = new PredictionRequest
{
    ChatflowId = "your-chatflow-id",
    Question = "Remember my name is John",
    OverrideConfig = overrideConfig
};

var response1 = await client.CreatePredictionAsync(request);

// Later in the conversation...
request = request with { Question = "What's my name?" };
var response2 = await client.CreatePredictionAsync(request);
// AI should respond with "John"
```

### With Conversation History

```csharp
var history = new List<ChatMessage>
{
    new ChatMessage { Role = "user", Content = "What's the capital of France?" },
    new ChatMessage { Role = "assistant", Content = "The capital of France is Paris." }
};

var request = new PredictionRequest
{
    ChatflowId = "your-chatflow-id",
    Question = "What's the population of that city?",
    History = history
};

var response = await client.CreatePredictionAsync(request);
```

## Chatflow Management

### List All Chatflows

```csharp
var chatflows = await client.GetChatflowsAsync();
foreach (var flow in chatflows)
{
    Console.WriteLine($"ID: {flow.Id}, Name: {flow.Name}, Deployed: {flow.Deployed}");
}
```

### Get Specific Chatflow

```csharp
var chatflow = await client.GetChatflowAsync("chatflow-id");
Console.WriteLine($"Chatflow: {chatflow.Name}");
```

### Create New Chatflow

```csharp
var newChatflow = new CreateChatflowRequest
{
    Name = "My New Chatflow",
    FlowData = "{}",  // JSON flow configuration
    Deployed = false,
    IsPublic = true,
    Type = ChatflowTypes.Chatflow
};

var created = await client.CreateChatflowAsync(newChatflow);
Console.WriteLine($"Created chatflow with ID: {created.Id}");
```

### Update Chatflow

```csharp
var update = new UpdateChatflowRequest
{
    Name = "Updated Chatflow Name",
    Deployed = true
};

var updated = await client.UpdateChatflowAsync("chatflow-id", update);
```

### Delete Chatflow

```csharp
await client.DeleteChatflowAsync("chatflow-id");
```

## Configuration Options

```csharp
var options = new FlowiseClientOptions
{
    BaseUrl = "https://your-flowise-instance.com",
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
    var response = await client.CreatePredictionAsync(request);
    Console.WriteLine(response.Text);
}
catch (FlowiseClientException ex)
{
    Console.WriteLine($"Flowise API error: {ex.Message}");
    if (ex.StatusCode.HasValue)
    {
        Console.WriteLine($"Status Code: {ex.StatusCode}");
        Console.WriteLine($"Response: {ex.ResponseContent}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## Health Check

```csharp
var isHealthy = await client.PingAsync();
if (isHealthy)
{
    Console.WriteLine("Flowise server is responding");
}
else
{
    Console.WriteLine("Flowise server is not responding");
}
```

## Override Configuration Helpers

Use the `OverrideConfigBuilder` for common configuration scenarios:

```csharp
// Session-only
var config1 = OverrideConfigBuilder.WithSessionId("user-123");

// Temperature-only
var config2 = OverrideConfigBuilder.WithTemperature(0.8);

// Max tokens-only
var config3 = OverrideConfigBuilder.WithMaxTokens(1000);

// Return source documents
var config4 = OverrideConfigBuilder.WithReturnSourceDocuments(true);

// Comprehensive configuration
var config5 = OverrideConfigBuilder.Create(
    sessionId: "user-123",
    temperature: 0.7,
    maxTokens: 500,
    returnSourceDocuments: true
);
```

## API Endpoints Covered

- **POST** `/api/v1/prediction/{chatflowId}` - Create prediction (with streaming support)
- **GET** `/chatflows` - List all chatflows
- **GET** `/chatflows/{id}` - Get chatflow by ID
- **GET** `/chatflows/apikey` - Get chatflow by API key
- **POST** `/chatflows` - Create new chatflow
- **PUT** `/chatflows/{id}` - Update chatflow
- **DELETE** `/chatflows/{id}` - Delete chatflow
- **GET** `/ping` - Health check

## Requirements

- .NET 9.0 or later
- RestSharp 112.0.0 or later
- System.Text.Json 9.0.0 or later

## License

This project is licensed under the MIT License.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
