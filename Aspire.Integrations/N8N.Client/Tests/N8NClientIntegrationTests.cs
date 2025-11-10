using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using N8N.Client;
using N8N.Client.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace N8N.Client.Tests
{
    /// <summary>
    /// Example integration test for N8NClient
    /// Note: This requires a running N8N instance with API access for proper testing
    /// </summary>
    public class N8NClientIntegrationTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<N8NClientIntegrationTests> _logger;

        public N8NClientIntegrationTests()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole());
            
            // Configure N8N client
            services.AddSingleton<N8NClientOptions>(new N8NClientOptions
            {
                BaseUrl = "http://localhost:5678", // Change this to your N8N instance
                ApiKey = "your-api-key-here",      // Change this to your actual API key
                TimeoutMilliseconds = 30000
            });
            
            services.AddTransient<IN8NClient, N8NClient>();
            
            _serviceProvider = services.BuildServiceProvider();
            _logger = _serviceProvider.GetRequiredService<ILogger<N8NClientIntegrationTests>>();
        }

        /// <summary>
        /// Test listing workflows
        /// </summary>
        public async Task<bool> TestListWorkflowsAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                var workflows = await client.GetWorkflowsAsync(new ListWorkflowsOptions
                {
                    Limit = 10
                });
                
                _logger.LogInformation("List workflows test: Found {Count} workflows", workflows.Data.Count());
                
                if (workflows.Data.Any())
                {
                    var firstWorkflow = workflows.Data.First();
                    _logger.LogInformation("First workflow: ID={Id}, Name={Name}, Active={Active}", 
                        firstWorkflow.Id, firstWorkflow.Name, firstWorkflow.Active);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list workflows");
                return false;
            }
        }

        /// <summary>
        /// Test getting a specific workflow (requires a workflow to exist)
        /// </summary>
        public async Task<bool> TestGetWorkflowAsync(string? workflowId = null)
        {
            if (string.IsNullOrEmpty(workflowId))
            {
                _logger.LogWarning("Skipping get workflow test - no workflow ID provided");
                return false;
            }

            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                var workflow = await client.GetWorkflowAsync(workflowId);
                
                _logger.LogInformation("Get workflow test: Retrieved workflow {Name} with {NodeCount} nodes", 
                    workflow.Name, workflow.Nodes.Count);
                
                return workflow.Id == workflowId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get workflow {WorkflowId}", workflowId);
                return false;
            }
        }

        /// <summary>
        /// Test workflow lifecycle (create, update, delete)
        /// </summary>
        public async Task<bool> TestWorkflowLifecycleAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                // Create workflow
                var createRequest = new CreateWorkflowRequest
                {
                    Name = $"Test Workflow {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                    Nodes = [
                        new Node
                        {
                            Id = "start-node",
                            Name = "Start",
                            Type = "n8n-nodes-base.start",
                            Position = [100, 200]
                        }
                    ],
                    Connections = [],
                    Settings = new WorkflowSettings
                    {
                        ExecutionTimeout = 3600
                    }
                };

                var created = await client.CreateWorkflowAsync(createRequest);
                _logger.LogInformation("Created workflow: {Name} (ID: {Id})", created.Name, created.Id);

                if (string.IsNullOrEmpty(created.Id))
                {
                    _logger.LogError("Created workflow has no ID");
                    return false;
                }

                try
                {
                    // Update workflow
                    var updateRequest = new UpdateWorkflowRequest
                    {
                        Name = $"{created.Name} - Updated"
                    };

                    var updated = await client.UpdateWorkflowAsync(created.Id, updateRequest);
                    _logger.LogInformation("Updated workflow name to: {Name}", updated.Name);

                    // Activate workflow
                    var activated = await client.ActivateWorkflowAsync(created.Id);
                    _logger.LogInformation("Activated workflow: {Active}", activated.Active);

                    // Deactivate workflow
                    var deactivated = await client.DeactivateWorkflowAsync(created.Id);
                    _logger.LogInformation("Deactivated workflow: {Active}", deactivated.Active);

                    return true;
                }
                finally
                {
                    // Clean up - delete the test workflow
                    try
                    {
                        await client.DeleteWorkflowAsync(created.Id);
                        _logger.LogInformation("Cleaned up test workflow: {Name}", created.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up test workflow {Id}", created.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed workflow lifecycle test");
                return false;
            }
        }

        /// <summary>
        /// Test listing executions
        /// </summary>
        public async Task<bool> TestListExecutionsAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                var executions = await client.GetExecutionsAsync(new ListExecutionsOptions
                {
                    Limit = 5,
                    IncludeData = false
                });
                
                _logger.LogInformation("List executions test: Found {Count} executions", executions.Data.Count());
                
                if (executions.Data.Any())
                {
                    var firstExecution = executions.Data.First();
                    _logger.LogInformation("First execution: ID={Id}, Status={Status}, WorkflowId={WorkflowId}", 
                        firstExecution.Id, firstExecution.Status, firstExecution.WorkflowId);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list executions");
                return false;
            }
        }

        /// <summary>
        /// Test tag management
        /// </summary>
        public async Task<bool> TestTagManagementAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                // Create tag
                var createTag = new Tag
                {
                    Name = $"TestTag-{DateTime.Now:yyyyMMdd-HHmmss}"
                };

                var created = await client.CreateTagAsync(createTag);
                _logger.LogInformation("Created tag: {Name} (ID: {Id})", created.Name, created.Id);

                if (string.IsNullOrEmpty(created.Id))
                {
                    _logger.LogError("Created tag has no ID");
                    return false;
                }

                try
                {
                    // Update tag
                    var updateTag = new Tag
                    {
                        Name = $"{created.Name}-Updated"
                    };

                    var updated = await client.UpdateTagAsync(created.Id, updateTag);
                    _logger.LogInformation("Updated tag name to: {Name}", updated.Name);

                    return true;
                }
                finally
                {
                    // Clean up - delete the test tag
                    try
                    {
                        await client.DeleteTagAsync(created.Id);
                        _logger.LogInformation("Cleaned up test tag: {Name}", created.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up test tag {Id}", created.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed tag management test");
                return false;
            }
        }

        /// <summary>
        /// Test variable management
        /// </summary>
        public async Task<bool> TestVariableManagementAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                // List existing variables
                var variables = await client.GetVariablesAsync();
                _logger.LogInformation("Found {Count} existing variables", variables.Data.Count());

                // Create variable
                var createVariable = new VariableCreateRequest
                {
                    Key = $"TEST_VAR_{DateTime.Now:yyyyMMddHHmmss}",
                    Value = "Test Value"
                };

                await client.CreateVariableAsync(createVariable);
                _logger.LogInformation("Created variable: {Key}", createVariable.Key);

                // Find our variable
                var updatedVariables = await client.GetVariablesAsync();
                var ourVariable = updatedVariables.Data.FirstOrDefault(v => v.Key == createVariable.Key);

                if (ourVariable == null)
                {
                    _logger.LogError("Could not find created variable");
                    return false;
                }

                try
                {
                    // Update variable
                    var updateVariable = new VariableCreateRequest
                    {
                        Key = ourVariable.Key,
                        Value = "Updated Test Value"
                    };

                    await client.UpdateVariableAsync(ourVariable.Id!, updateVariable);
                    _logger.LogInformation("Updated variable: {Key}", ourVariable.Key);

                    return true;
                }
                finally
                {
                    // Clean up - delete the test variable
                    try
                    {
                        await client.DeleteVariableAsync(ourVariable.Id!);
                        _logger.LogInformation("Cleaned up test variable: {Key}", ourVariable.Key);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to clean up test variable {Id}", ourVariable.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed variable management test");
                return false;
            }
        }

        /// <summary>
        /// Test error handling with invalid requests
        /// </summary>
        public async Task<bool> TestErrorHandlingAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                // Try to get a non-existent workflow
                await client.GetWorkflowAsync("non-existent-workflow-id");
                
                _logger.LogWarning("Expected error but request succeeded");
                return false; // Should have thrown an exception
            }
            catch (N8NClientException ex)
            {
                _logger.LogInformation("Error handling test successful. Caught N8NClientException: {Message}", ex.Message);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception type in error handling test");
                return false;
            }
        }

        /// <summary>
        /// Test security audit generation
        /// </summary>
        public async Task<bool> TestGenerateAuditAsync()
        {
            using var client = _serviceProvider.GetRequiredService<IN8NClient>();
            
            try
            {
                var auditRequest = new GenerateAuditRequest
                {
                    AdditionalOptions = new AuditAdditionalOptions
                    {
                        DaysAbandonedWorkflow = 30,
                        Categories = [AuditCategories.Instance]
                    }
                };

                var audit = await client.GenerateAuditAsync(auditRequest);
                
                _logger.LogInformation("Audit test successful. Generated audit report");
                
                if (audit.InstanceRiskReport != null)
                {
                    _logger.LogInformation("Instance Risk Report: {SectionCount} sections", 
                        audit.InstanceRiskReport.Sections?.Count ?? 0);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate audit");
                return false;
            }
        }

        /// <summary>
        /// Run all tests
        /// </summary>
        public async Task RunAllTestsAsync(string? testWorkflowId = null)
        {
            _logger.LogInformation("Starting N8N Client Integration Tests");
            _logger.LogInformation("====================================");

            var results = new Dictionary<string, bool>();

            // Test 1: List Workflows
            results["ListWorkflows"] = await TestListWorkflowsAsync();

            // Test 2: Get Workflow (only if workflow ID provided)
            if (!string.IsNullOrEmpty(testWorkflowId))
            {
                results["GetWorkflow"] = await TestGetWorkflowAsync(testWorkflowId);
            }

            // Test 3: Workflow Lifecycle
            results["WorkflowLifecycle"] = await TestWorkflowLifecycleAsync();

            // Test 4: List Executions
            results["ListExecutions"] = await TestListExecutionsAsync();

            // Test 5: Tag Management
            results["TagManagement"] = await TestTagManagementAsync();

            // Test 6: Variable Management
            results["VariableManagement"] = await TestVariableManagementAsync();

            // Test 7: Error Handling
            results["ErrorHandling"] = await TestErrorHandlingAsync();

            // Test 8: Generate Audit
            results["GenerateAudit"] = await TestGenerateAuditAsync();

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
    /// Usage: dotnet run [workflow-id]
    /// </summary>
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var testWorkflowId = args.Length > 0 ? args[0] : null;
            
            var tests = new N8NClientIntegrationTests();
            await tests.RunAllTestsAsync(testWorkflowId);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
