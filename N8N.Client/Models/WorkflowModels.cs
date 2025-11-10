using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace N8N.Client.Models
{
    /// <summary>
    /// Workflow model
    /// </summary>
    public class Workflow
    {
        /// <summary>
        /// Workflow ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Workflow name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Whether the workflow is active (read-only)
        /// </summary>
        [JsonPropertyName("active")]
        public bool? Active { get; set; }

        /// <summary>
        /// Creation timestamp (read-only)
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (read-only)
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Array of nodes in the workflow
        /// </summary>
        [JsonPropertyName("nodes")]
        public required IList<Node> Nodes { get; set; } = [];

        /// <summary>
        /// Connections between nodes
        /// </summary>
        [JsonPropertyName("connections")]
        public required Dictionary<string, object> Connections { get; set; } = [];

        /// <summary>
        /// Workflow settings
        /// </summary>
        [JsonPropertyName("settings")]
        public required WorkflowSettings Settings { get; set; }

        /// <summary>
        /// Static data for the workflow
        /// </summary>
        [JsonPropertyName("staticData")]
        public object? StaticData { get; set; }

        /// <summary>
        /// Tags associated with the workflow (read-only)
        /// </summary>
        [JsonPropertyName("tags")]
        public IList<Tag>? Tags { get; set; }

        /// <summary>
        /// Sharing information for the workflow
        /// </summary>
        [JsonPropertyName("shared")]
        public IList<SharedWorkflow>? Shared { get; set; }
    }

    /// <summary>
    /// Node model
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Node ID
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        /// Node name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Webhook ID (if applicable)
        /// </summary>
        [JsonPropertyName("webhookId")]
        public string? WebhookId { get; set; }

        /// <summary>
        /// Whether the node is disabled
        /// </summary>
        [JsonPropertyName("disabled")]
        public bool? Disabled { get; set; }

        /// <summary>
        /// Whether notes should be shown in flow
        /// </summary>
        [JsonPropertyName("notesInFlow")]
        public bool? NotesInFlow { get; set; }

        /// <summary>
        /// Notes for the node
        /// </summary>
        [JsonPropertyName("notes")]
        public string? Notes { get; set; }

        /// <summary>
        /// Node type (e.g., "n8n-nodes-base.jira")
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// Type version
        /// </summary>
        [JsonPropertyName("typeVersion")]
        public int? TypeVersion { get; set; }

        /// <summary>
        /// Whether to execute only once
        /// </summary>
        [JsonPropertyName("executeOnce")]
        public bool? ExecuteOnce { get; set; }

        /// <summary>
        /// Whether to always output data
        /// </summary>
        [JsonPropertyName("alwaysOutputData")]
        public bool? AlwaysOutputData { get; set; }

        /// <summary>
        /// Whether to retry on failure
        /// </summary>
        [JsonPropertyName("retryOnFail")]
        public bool? RetryOnFail { get; set; }

        /// <summary>
        /// Maximum number of retry attempts
        /// </summary>
        [JsonPropertyName("maxTries")]
        public int? MaxTries { get; set; }

        /// <summary>
        /// Wait time between retries
        /// </summary>
        [JsonPropertyName("waitBetweenTries")]
        public int? WaitBetweenTries { get; set; }

        /// <summary>
        /// Whether to continue on failure (deprecated, use OnError instead)
        /// </summary>
        [JsonPropertyName("continueOnFail")]
        [Obsolete("Use OnError instead")]
        public bool? ContinueOnFail { get; set; }

        /// <summary>
        /// Action to take on error
        /// </summary>
        [JsonPropertyName("onError")]
        public string? OnError { get; set; }

        /// <summary>
        /// Node position [x, y]
        /// </summary>
        [JsonPropertyName("position")]
        public IList<double>? Position { get; set; }

        /// <summary>
        /// Node parameters
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, object>? Parameters { get; set; }

        /// <summary>
        /// Node credentials
        /// </summary>
        [JsonPropertyName("credentials")]
        public Dictionary<string, object>? Credentials { get; set; }

        /// <summary>
        /// Creation timestamp (read-only)
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (read-only)
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Workflow settings
    /// </summary>
    public class WorkflowSettings
    {
        /// <summary>
        /// Whether to save execution progress
        /// </summary>
        [JsonPropertyName("saveExecutionProgress")]
        public bool? SaveExecutionProgress { get; set; }

        /// <summary>
        /// Whether to save manual executions
        /// </summary>
        [JsonPropertyName("saveManualExecutions")]
        public bool? SaveManualExecutions { get; set; }

        /// <summary>
        /// Data save option for error executions (all, none)
        /// </summary>
        [JsonPropertyName("saveDataErrorExecution")]
        public string? SaveDataErrorExecution { get; set; }

        /// <summary>
        /// Data save option for success executions (all, none)
        /// </summary>
        [JsonPropertyName("saveDataSuccessExecution")]
        public string? SaveDataSuccessExecution { get; set; }

        /// <summary>
        /// Execution timeout in seconds (max 3600)
        /// </summary>
        [JsonPropertyName("executionTimeout")]
        public int? ExecutionTimeout { get; set; }

        /// <summary>
        /// ID of the workflow that contains the error trigger node
        /// </summary>
        [JsonPropertyName("errorWorkflow")]
        public string? ErrorWorkflow { get; set; }

        /// <summary>
        /// Timezone (e.g., "America/New_York")
        /// </summary>
        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        /// <summary>
        /// Execution order (e.g., "v1")
        /// </summary>
        [JsonPropertyName("executionOrder")]
        public string? ExecutionOrder { get; set; }
    }

    /// <summary>
    /// Shared workflow information
    /// </summary>
    public class SharedWorkflow
    {
        /// <summary>
        /// Role for this sharing relationship
        /// </summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        /// <summary>
        /// Workflow ID
        /// </summary>
        [JsonPropertyName("workflowId")]
        public string? WorkflowId { get; set; }

        /// <summary>
        /// Project ID
        /// </summary>
        [JsonPropertyName("projectId")]
        public string? ProjectId { get; set; }

        /// <summary>
        /// Project information
        /// </summary>
        [JsonPropertyName("project")]
        public Project? Project { get; set; }

        /// <summary>
        /// Creation timestamp (read-only)
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (read-only)
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Request model for creating workflows
    /// </summary>
    public class CreateWorkflowRequest
    {
        /// <summary>
        /// Workflow name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Array of nodes in the workflow
        /// </summary>
        [JsonPropertyName("nodes")]
        public IList<Node> Nodes { get; set; } = [];

        /// <summary>
        /// Connections between nodes
        /// </summary>
        [JsonPropertyName("connections")]
        public Dictionary<string, object> Connections { get; set; } = [];

        /// <summary>
        /// Workflow settings
        /// </summary>
        [JsonPropertyName("settings")]
        public WorkflowSettings? Settings { get; set; }

        /// <summary>
        /// Static data for the workflow
        /// </summary>
        [JsonPropertyName("staticData")]
        public object? StaticData { get; set; }
    }

    /// <summary>
    /// Request model for updating workflows
    /// </summary>
    public class UpdateWorkflowRequest
    {
        /// <summary>
        /// Workflow name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Array of nodes in the workflow
        /// </summary>
        [JsonPropertyName("nodes")]
        public IList<Node>? Nodes { get; set; }

        /// <summary>
        /// Connections between nodes
        /// </summary>
        [JsonPropertyName("connections")]
        public Dictionary<string, object>? Connections { get; set; }

        /// <summary>
        /// Workflow settings
        /// </summary>
        [JsonPropertyName("settings")]
        public WorkflowSettings? Settings { get; set; }

        /// <summary>
        /// Static data for the workflow
        /// </summary>
        [JsonPropertyName("staticData")]
        public object? StaticData { get; set; }
    }

    /// <summary>
    /// Request for transferring a workflow to another project
    /// </summary>
    public class TransferWorkflowRequest
    {
        /// <summary>
        /// The ID of the destination project
        /// </summary>
        [JsonPropertyName("destinationProjectId")]
        public required string DestinationProjectId { get; set; }
    }

    /// <summary>
    /// Request for updating workflow tags
    /// </summary>
    public class UpdateWorkflowTagsRequest
    {
        /// <summary>
        /// List of tag IDs
        /// </summary>
        [JsonPropertyName("tagIds")]
        public required IList<TagIdReference> TagIds { get; set; }
    }

    /// <summary>
    /// Tag ID reference for workflows
    /// </summary>
    public class TagIdReference
    {
        /// <summary>
        /// Tag ID
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }

    /// <summary>
    /// Workflow list response
    /// </summary>
    public class WorkflowListResponse : PaginatedResponse<Workflow>
    {
        /// <summary>
        /// The workflows for this page
        /// </summary>
        [JsonPropertyName("data")]
        public new required IEnumerable<Workflow> Data { get; set; }

        /// <summary>
        /// Cursor for the next page
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public new string? NextCursor { get; set; }
    }

    /// <summary>
    /// Options for listing workflows
    /// </summary>
    public class ListWorkflowsOptions : PaginationOptions
    {
        /// <summary>
        /// Filter by active status
        /// </summary>
        public bool? Active { get; set; }

        /// <summary>
        /// Filter by tags (comma-separated)
        /// </summary>
        public string? Tags { get; set; }

        /// <summary>
        /// Filter by workflow name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Filter by project ID
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// Whether to exclude pinned data
        /// </summary>
        public bool? ExcludePinnedData { get; set; }
    }

    /// <summary>
    /// Options for getting a single workflow
    /// </summary>
    public class GetWorkflowOptions
    {
        /// <summary>
        /// Whether to exclude pinned data
        /// </summary>
        public bool? ExcludePinnedData { get; set; }
    }
}
