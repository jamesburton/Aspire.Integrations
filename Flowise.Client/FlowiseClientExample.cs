using System;
using System.Linq;
using System.Threading.Tasks;
using Flowise.Client;
using Flowise.Client.Models;

namespace Flowise.Client.Example
{
    /// <summary>
    /// Example console application demonstrating Flowise client usage
    /// </summary>
    public class FlowiseClientExample
    {
        private const string DefaultBaseUrl = "http://localhost:3000";
        private const string DefaultChatflowId = "your-chatflow-id-here";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Flowise Client Example");
            Console.WriteLine("======================");

            // You can pass base URL and chatflow ID as arguments
            var baseUrl = args.Length > 0 ? args[0] : DefaultBaseUrl;
            var chatflowId = args.Length > 1 ? args[1] : DefaultChatflowId;
            var apiKey = args.Length > 2 ? args[2] : null;

            Console.WriteLine($"Base URL: {baseUrl}");
            Console.WriteLine($"Chatflow ID: {chatflowId}");
            Console.WriteLine($"API Key: {(string.IsNullOrEmpty(apiKey) ? "Not provided" : "***")}");
            Console.WriteLine();

            // Create client options
            var options = new FlowiseClientOptions
            {
                BaseUrl = baseUrl,
                ApiKey = apiKey
            };

            using var client = new FlowiseClient(options);

            try
            {
                // Test server connectivity
                Console.WriteLine("Testing server connectivity...");
                var isAlive = await client.PingAsync();
                Console.WriteLine($"Server is {(isAlive ? "responding" : "not responding")}");
                
                if (!isAlive)
                {
                    Console.WriteLine("Cannot connect to Flowise server. Please check your configuration.");
                    return;
                }

                Console.WriteLine();

                // Example 1: Simple chat
                await SimpleChatExample(client, chatflowId);

                // Example 2: Session-based conversation
                await SessionBasedConversationExample(client, chatflowId);

                // Example 3: Streaming response
                await StreamingResponseExample(client, chatflowId);

                // Example 4: Chat with configuration override
                await ConfigurationOverrideExample(client, chatflowId);

                // Example 5: List chatflows
                await ListChatflowsExample(client);
            }
            catch (FlowiseClientException ex)
            {
                Console.WriteLine($"Flowise API Error: {ex.Message}");
                if (ex.StatusCode.HasValue)
                {
                    Console.WriteLine($"Status Code: {ex.StatusCode}");
                    Console.WriteLine($"Response: {ex.ResponseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task SimpleChatExample(IFlowiseClient client, string chatflowId)
        {
            Console.WriteLine("=== Example 1: Simple Chat ===");

            var request = new PredictionRequest
            {
                ChatflowId = chatflowId,
                Question = "Hello! Can you introduce yourself?"
            };

            var response = await client.CreatePredictionAsync(request);
            Console.WriteLine($"Q: {request.Question}");
            Console.WriteLine($"A: {response.Text}");
            Console.WriteLine();
        }

        private static async Task SessionBasedConversationExample(IFlowiseClient client, string chatflowId)
        {
            Console.WriteLine("=== Example 2: Session-based Conversation ===");

            var sessionId = Guid.NewGuid().ToString();
            var overrideConfig = OverrideConfigBuilder.WithSessionId(sessionId);

            // First message
            var request1 = new PredictionRequest
            {
                ChatflowId = chatflowId,
                Question = "My name is John and I love pizza. Please remember this.",
                OverrideConfig = overrideConfig
            };

            var response1 = await client.CreatePredictionAsync(request1);
            Console.WriteLine($"Q1: {request1.Question}");
            Console.WriteLine($"A1: {response1.Text}");

            // Second message in same session
            var request2 = new PredictionRequest
            {
                ChatflowId = chatflowId,
                Question = "What's my name and what food do I like?",
                OverrideConfig = overrideConfig
            };

            var response2 = await client.CreatePredictionAsync(request2);
            Console.WriteLine($"Q2: {request2.Question}");
            Console.WriteLine($"A2: {response2.Text}");
            Console.WriteLine();
        }

        private static async Task StreamingResponseExample(IFlowiseClient client, string chatflowId)
        {
            Console.WriteLine("=== Example 3: Streaming Response ===");

            var request = new PredictionRequest
            {
                ChatflowId = chatflowId,
                Question = "Tell me a brief story about artificial intelligence.",
                Streaming = true
            };

            Console.WriteLine($"Q: {request.Question}");
            Console.Write("A: ");

            await foreach (var chunk in client.CreateStreamingPredictionAsync(request))
            {
                Console.Write(chunk);
                // Add small delay to simulate real-time streaming
                await Task.Delay(50);
            }

            Console.WriteLine("\n");
        }

        private static async Task ConfigurationOverrideExample(IFlowiseClient client, string chatflowId)
        {
            Console.WriteLine("=== Example 4: Configuration Override ===");

            var overrideConfig = OverrideConfigBuilder.Create(
                sessionId: "config-example-session",
                temperature: 0.1, // Low temperature for more deterministic responses
                maxTokens: 100,   // Limit response length
                returnSourceDocuments: true
            );

            var request = new PredictionRequest
            {
                ChatflowId = chatflowId,
                Question = "Explain machine learning in simple terms.",
                OverrideConfig = overrideConfig
            };

            var response = await client.CreatePredictionAsync(request);
            Console.WriteLine($"Q: {request.Question}");
            Console.WriteLine($"A: {response.Text}");

            if (response.SourceDocuments?.Any() == true)
            {
                Console.WriteLine("\nSource Documents:");
                foreach (var doc in response.SourceDocuments)
                {
                    var content = doc.PageContent ?? string.Empty;
                    var preview = content.Length > 100 ? content[..100] : content;
                    Console.WriteLine($"- {preview}...");
                }
            }

            Console.WriteLine();
        }

        private static async Task ListChatflowsExample(IFlowiseClient client)
        {
            Console.WriteLine("=== Example 5: List Chatflows ===");

            try
            {
                var chatflows = await client.GetChatflowsAsync();
                
                if (chatflows.Any())
                {
                    Console.WriteLine("Available Chatflows:");
                    foreach (var flow in chatflows)
                    {
                        Console.WriteLine($"- ID: {flow.Id}");
                        Console.WriteLine($"  Name: {flow.Name}");
                        Console.WriteLine($"  Type: {flow.Type}");
                        Console.WriteLine($"  Deployed: {flow.Deployed}");
                        Console.WriteLine($"  Public: {flow.IsPublic}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("No chatflows found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retrieve chatflows: {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
