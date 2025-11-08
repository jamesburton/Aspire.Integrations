using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flowise.Client;
using Flowise.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flowise.Client.Tests
{
    /// <summary>
    /// Example integration test for FlowiseClient
    /// Note: This requires a running Flowise instance for proper testing
    /// </summary>
    public class FlowiseClientIntegrationTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FlowiseClientIntegrationTests> _logger;

        public FlowiseClientIntegrationTests()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            
            // Configure Flowise client
            services.AddSingleton<FlowiseClientOptions>(new FlowiseClientOptions
            {
                BaseUrl = "http://localhost:3000",
                ApiKey = null, // Add your API key here if needed
                TimeoutMilliseconds = 30000
            });
            
            services.AddTransient<IFlowiseClient, FlowiseClient>();
            
            _serviceProvider = services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<FlowiseClientIntegrationTests>>();
        }

        /// <summary>
        /// Test basic connectivity to Flowise server
        /// </summary>
        public async Task<bool> TestConnectivityAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IFlowiseClient>();
            
            try
            {
                var isAlive = await client.PingAsync();
                _logger.LogInformation("Connectivity test: {Result}", isAlive ? "SUCCESS" : "FAILED");
                return isAlive;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connectivity test failed");
                return false;
            }
        }

        /// <summary>
        /// Test listing chatflows
        /// </summary>
        public async Task<bool> TestListChatflowsAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IFlowiseClient>();
            
            try
            {
                var chatflows = await client.GetChatflowsAsync();
                _logger.LogInformation("Found {Count} chatflows", chatflows?.Count() ?? 0);
                
                if (chatflows?.Any() == true)
                {
                    foreach (var flow in chatflows.Take(3)) // Log first 3
                    {
                        _logger.LogInformation("Chatflow: ID={Id}, Name={Name}, Deployed={Deployed}", 
                            flow.Id, flow.Name, flow.Deployed);
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list chatflows");
                return false;
            }
        }

        /// <summary>
        /// Test creating a prediction (requires valid chatflow ID)
        /// </summary>
        public async Task<bool> TestCreatePredictionAsync(string chatflowId)
        {
            if (string.IsNullOrEmpty(chatflowId))
            {
                _logger.LogWarning("Skipping prediction test - no chatflow ID provided");
                return false;
            }

            using var client = _serviceProvider.GetRequiredService<IFlowiseClient>();
            
            try
            {
                var request = new PredictionRequest
                {
                    ChatflowId = chatflowId,
                    Question = "Hello! This is a test message from the Flowise client library."
                };

                var response = await client.CreatePredictionAsync(request);
                
                _logger.LogInformation("Prediction successful. Response length: {Length} characters", 
                    response.Text?.Length ?? 0);
                
                if (!string.IsNullOrEmpty(response.Text))
                {
                    _logger.LogInformation("Response preview: {Preview}", 
                        response.Text.Substring(0, Math.Min(100, response.Text.Length)));
                }
                
                return !string.IsNullOrEmpty(response.Text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create prediction");
                return false;
            }
        }

        /// <summary>
        /// Test streaming prediction (requires valid chatflow ID)
        /// </summary>
        public async Task<bool> TestStreamingPredictionAsync(string chatflowId)
        {
            if (string.IsNullOrEmpty(chatflowId))
            {
                _logger.LogWarning("Skipping streaming test - no chatflow ID provided");
                return false;
            }

            using var client = _serviceProvider.GetRequiredService<IFlowiseClient>();
            
            try
            {
                var request = new PredictionRequest
                {
                    ChatflowId = chatflowId,
                    Question = "Tell me a very brief joke.",
                    Streaming = true
                };

                var chunks = new List<string>();
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));

                await foreach (var chunk in client.CreateStreamingPredictionAsync(request, cancellationTokenSource.Token))
                {
                    chunks.Add(chunk);
                    
                    // Stop after reasonable amount of chunks to avoid infinite streams
                    if (chunks.Count > 50)
                    {
                        cancellationTokenSource.Cancel();
                        break;
                    }
                }

                _logger.LogInformation("Streaming test completed. Received {ChunkCount} chunks", chunks.Count);
                
                return chunks.Count > 0;
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Streaming test completed (cancelled after timeout/limit)");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create streaming prediction");
                return false;
            }
        }

        /// <summary>
        /// Test error handling with invalid chatflow ID
        /// </summary>
        public async Task<bool> TestErrorHandlingAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IFlowiseClient>();
            
            try
            {
                var request = new PredictionRequest
                {
                    ChatflowId = "invalid-chatflow-id",
                    Question = "This should fail"
                };

                await client.CreatePredictionAsync(request);
                
                _logger.LogWarning("Expected error but request succeeded");
                return false; // Should have thrown an exception
            }
            catch (FlowiseClientException ex)
            {
                _logger.LogInformation("Error handling test successful. Caught FlowiseClientException: {Message}", ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception type in error handling test");
                return false;
            }
        }

        /// <summary>
        /// Run all tests
        /// </summary>
        public async Task RunAllTestsAsync(string? testChatflowId = null)
        {
            _logger.LogInformation("Starting Flowise Client Integration Tests");
            _logger.LogInformation("=========================================");

            var results = new Dictionary<string, bool>();

            // Test 1: Connectivity
            results["Connectivity"] = await TestConnectivityAsync();

            // Test 2: List Chatflows
            results["ListChatflows"] = await TestListChatflowsAsync();

            // Test 3: Create Prediction (only if chatflow ID provided)
            if (!string.IsNullOrEmpty(testChatflowId))
            {
                results["CreatePrediction"] = await TestCreatePredictionAsync(testChatflowId);
                results["StreamingPrediction"] = await TestStreamingPredictionAsync(testChatflowId);
            }

            // Test 4: Error Handling
            results["ErrorHandling"] = await TestErrorHandlingAsync();

            // Report results
            _logger.LogInformation("Test Results:");
            _logger.LogInformation("=============");
            
            foreach (var result in results)
            {
                var status = result.Value ? "PASSED" : "FAILED";
                _logger.LogInformation("{TestName}: {Status}", result.Key, status);
            }

            var passedCount = results.Values.Count(x => x);
            var totalCount = results.Count;
            
            _logger.LogInformation("Summary: {PassedCount}/{TotalCount} tests passed", passedCount, totalCount);
        }
    }

    /// <summary>
    /// Console application to run the integration tests
    /// Usage: dotnet run [chatflow-id]
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var testChatflowId = args.Length > 0 ? args[0] : null;
            
            var tests = new FlowiseClientIntegrationTests();
            await tests.RunAllTestsAsync(testChatflowId);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
