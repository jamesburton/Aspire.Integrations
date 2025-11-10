using System;
using System.Linq;
using System.Threading.Tasks;
using N8N.Client;
using N8N.Client.Models;

namespace N8N.Client.Example
{
    /// <summary>
    /// Example console application demonstrating N8N client usage
    /// </summary>
    public class N8NClientExample
    {
        private const string DefaultBaseUrl = "http://localhost:5678";
        private const string DefaultApiKey = "your-api-key-here";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("N8N Client Example");
            Console.WriteLine("==================");

            // You can pass base URL and API key as arguments
            var baseUrl = args.Length > 0 ? args[0] : DefaultBaseUrl;
            var apiKey = args.Length > 1 ? args[1] : DefaultApiKey;

            Console.WriteLine($"Base URL: {baseUrl}");
            Console.WriteLine($"API Key: {(apiKey == DefaultApiKey ? "Default (change this!)" : "***")}");
            Console.WriteLine();

            // Create client options
            var options = new N8NClientOptions
            {
                BaseUrl = baseUrl,
                ApiKey = apiKey
            };

            using var client = new N8NClient(options);

            try
            {
                // Example 1: List workflows
                await ListWorkflowsExample(client);

                // Example 2: List executions
                await ListExecutionsExample(client);

                // Example 3: Manage tags
                await ManageTagsExample(client);

                // Example 4: Manage variables
                await ManageVariablesExample(client);

                // Example 5: User management (Enterprise feature)
                await UserManagementExample(client);

                // Example 6: Generate security audit
                await GenerateAuditExample(client);

                // Example 7: Workflow lifecycle
                await WorkflowLifecycleExample(client);
            }
            catch (N8NClientException ex)
            {
                Console.WriteLine($"N8N API Error: {ex.Message}");
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
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static async Task ListWorkflowsExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 1: List Workflows ===");

            try
            {
                var options = new ListWorkflowsOptions
                {
                    Limit = 10,
                    Active = null // Get both active and inactive
                };

                var workflows = await client.GetWorkflowsAsync(options);
                
                if (workflows.Data.Any())
                {
                    Console.WriteLine($"Found {workflows.Data.Count()} workflows:");
                    foreach (var workflow in workflows.Data)
                    {
                        Console.WriteLine($"- ID: {workflow.Id}");
                        Console.WriteLine($"  Name: {workflow.Name}");
                        Console.WriteLine($"  Active: {workflow.Active}");
                        Console.WriteLine($"  Nodes: {workflow.Nodes.Count}");
                        Console.WriteLine($"  Created: {workflow.CreatedAt:yyyy-MM-dd}");
                        Console.WriteLine();
                    }
                    
                    if (!string.IsNullOrEmpty(workflows.NextCursor))
                    {
                        Console.WriteLine($"Next cursor available: {workflows.NextCursor[..20]}...");
                    }
                }
                else
                {
                    Console.WriteLine("No workflows found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retrieve workflows: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static async Task ListExecutionsExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 2: List Recent Executions ===");

            try
            {
                var options = new ListExecutionsOptions
                {
                    Limit = 5,
                    IncludeData = false // Exclude data for performance
                };

                var executions = await client.GetExecutionsAsync(options);
                
                if (executions.Data.Any())
                {
                    Console.WriteLine($"Found {executions.Data.Count()} recent executions:");
                    foreach (var execution in executions.Data)
                    {
                        Console.WriteLine($"- Execution ID: {execution.Id}");
                        Console.WriteLine($"  Workflow ID: {execution.WorkflowId}");
                        Console.WriteLine($"  Status: {execution.Status}");
                        Console.WriteLine($"  Mode: {execution.Mode}");
                        Console.WriteLine($"  Started: {execution.StartedAt:yyyy-MM-dd HH:mm:ss}");
                        if (execution.StoppedAt.HasValue)
                        {
                            var duration = execution.StoppedAt.Value - execution.StartedAt;
                            Console.WriteLine($"  Duration: {duration.TotalSeconds:F1} seconds");
                        }
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("No executions found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not retrieve executions: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static async Task ManageTagsExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 3: Manage Tags ===");

            try
            {
                // List existing tags
                var tags = await client.GetTagsAsync();
                Console.WriteLine($"Existing tags: {tags.Data.Count()}");

                // Create a new tag
                var newTag = new Tag
                {
                    Name = $"Example-{DateTime.Now:yyyyMMdd-HHmmss}"
                };

                var createdTag = await client.CreateTagAsync(newTag);
                Console.WriteLine($"Created tag: {createdTag.Name} (ID: {createdTag.Id})");

                // Update the tag
                if (!string.IsNullOrEmpty(createdTag.Id))
                {
                    var updateTag = new Tag
                    {
                        Name = $"{createdTag.Name}-Updated"
                    };

                    var updatedTag = await client.UpdateTagAsync(createdTag.Id, updateTag);
                    Console.WriteLine($"Updated tag name to: {updatedTag.Name}");

                    // Delete the tag
                    await client.DeleteTagAsync(createdTag.Id);
                    Console.WriteLine($"Deleted tag: {updatedTag.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tag management error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static async Task ManageVariablesExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 4: Manage Variables ===");

            try
            {
                // List existing variables
                var variables = await client.GetVariablesAsync();
                Console.WriteLine($"Existing variables: {variables.Data.Count()}");

                // Create a new variable
                var newVariable = new VariableCreateRequest
                {
                    Key = $"EXAMPLE_VAR_{DateTime.Now:yyyyMMddHHmmss}",
                    Value = "Example Value"
                };

                await client.CreateVariableAsync(newVariable);
                Console.WriteLine($"Created variable: {newVariable.Key}");

                // List variables again to see our new one
                var updatedVariables = await client.GetVariablesAsync();
                var ourVariable = updatedVariables.Data.FirstOrDefault(v => v.Key == newVariable.Key);
                
                if (ourVariable != null)
                {
                    Console.WriteLine($"Found our variable: {ourVariable.Key} = {ourVariable.Value}");

                    // Update the variable
                    var updateVariable = new VariableCreateRequest
                    {
                        Key = ourVariable.Key,
                        Value = "Updated Value"
                    };

                    await client.UpdateVariableAsync(ourVariable.Id!, updateVariable);
                    Console.WriteLine($"Updated variable value");

                    // Delete the variable
                    await client.DeleteVariableAsync(ourVariable.Id!);
                    Console.WriteLine($"Deleted variable: {ourVariable.Key}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Variable management error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static async Task UserManagementExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 5: User Management (Enterprise) ===");

            try
            {
                // List users (Enterprise feature)
                var users = await client.GetUsersAsync(new ListUsersOptions
                {
                    IncludeRole = true,
                    Limit = 10
                });

                if (users.Data.Any())
                {
                    Console.WriteLine($"Found {users.Data.Count()} users:");
                    foreach (var user in users.Data)
                    {
                        Console.WriteLine($"- Email: {user.Email}");
                        Console.WriteLine($"  Name: {user.FirstName} {user.LastName}");
                        Console.WriteLine($"  Role: {user.Role}");
                        Console.WriteLine($"  Pending: {user.IsPending}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("No users found (may require enterprise license).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"User management error (may require enterprise license): {ex.Message}");
            }

            Console.WriteLine();
        }

        private static async Task GenerateAuditExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 6: Generate Security Audit ===");

            try
            {
                var auditRequest = new GenerateAuditRequest
                {
                    AdditionalOptions = new AuditAdditionalOptions
                    {
                        DaysAbandonedWorkflow = 30,
                        Categories = [
                            AuditCategories.Credentials,
                            AuditCategories.Instance
                        ]
                    }
                };

                var audit = await client.GenerateAuditAsync(auditRequest);
                
                Console.WriteLine("Audit completed. Summary:");
                
                if (audit.CredentialsRiskReport != null)
                {
                    Console.WriteLine($"- Credentials Risk: {audit.CredentialsRiskReport.Sections?.Count ?? 0} issues");
                }
                if (audit.DatabaseRiskReport != null)
                {
                    Console.WriteLine($"- Database Risk: {audit.DatabaseRiskReport.Sections?.Count ?? 0} issues");
                }
                if (audit.FilesystemRiskReport != null)
                {
                    Console.WriteLine($"- Filesystem Risk: {audit.FilesystemRiskReport.Sections?.Count ?? 0} issues");
                }
                if (audit.NodesRiskReport != null)
                {
                    Console.WriteLine($"- Nodes Risk: {audit.NodesRiskReport.Sections?.Count ?? 0} issues");
                }
                if (audit.InstanceRiskReport != null)
                {
                    Console.WriteLine($"- Instance Risk: {audit.InstanceRiskReport.Sections?.Count ?? 0} issues");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Audit generation error: {ex.Message}");
            }

            Console.WriteLine();
        }

        private static async Task WorkflowLifecycleExample(IN8NClient client)
        {
            Console.WriteLine("=== Example 7: Workflow Lifecycle ===");

            try
            {
                // Create a simple workflow
                var workflow = new CreateWorkflowRequest
                {
                    Name = $"Example Workflow {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
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
                        SaveExecutionProgress = false,
                        ExecutionTimeout = 3600
                    }
                };

                var createdWorkflow = await client.CreateWorkflowAsync(workflow);
                Console.WriteLine($"Created workflow: {createdWorkflow.Name} (ID: {createdWorkflow.Id})");

                if (!string.IsNullOrEmpty(createdWorkflow.Id))
                {
                    // Get the workflow
                    var retrievedWorkflow = await client.GetWorkflowAsync(createdWorkflow.Id);
                    Console.WriteLine($"Retrieved workflow: {retrievedWorkflow.Name}");

                    // Activate the workflow
                    var activatedWorkflow = await client.ActivateWorkflowAsync(createdWorkflow.Id);
                    Console.WriteLine($"Workflow active: {activatedWorkflow.Active}");

                    // Deactivate the workflow
                    var deactivatedWorkflow = await client.DeactivateWorkflowAsync(createdWorkflow.Id);
                    Console.WriteLine($"Workflow active: {deactivatedWorkflow.Active}");

                    // Update the workflow
                    var updateRequest = new UpdateWorkflowRequest
                    {
                        Name = $"{createdWorkflow.Name} - Updated"
                    };

                    var updatedWorkflow = await client.UpdateWorkflowAsync(createdWorkflow.Id, updateRequest);
                    Console.WriteLine($"Updated workflow name: {updatedWorkflow.Name}");

                    // Delete the workflow
                    await client.DeleteWorkflowAsync(createdWorkflow.Id);
                    Console.WriteLine($"Deleted workflow: {updatedWorkflow.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Workflow lifecycle error: {ex.Message}");
            }

            Console.WriteLine();
        }
    }
}
