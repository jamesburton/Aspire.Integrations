using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace N8N.Client.Models
{
    /// <summary>
    /// Audit request model
    /// </summary>
    public class GenerateAuditRequest
    {
        /// <summary>
        /// Additional options for the audit
        /// </summary>
        [JsonPropertyName("additionalOptions")]
        public AuditAdditionalOptions? AdditionalOptions { get; set; }
    }

    /// <summary>
    /// Additional options for audit generation
    /// </summary>
    public class AuditAdditionalOptions
    {
        /// <summary>
        /// Days for a workflow to be considered abandoned if not executed
        /// </summary>
        [JsonPropertyName("daysAbandonedWorkflow")]
        public int? DaysAbandonedWorkflow { get; set; }

        /// <summary>
        /// Categories to include in the audit
        /// </summary>
        [JsonPropertyName("categories")]
        public IList<string>? Categories { get; set; }
    }

    /// <summary>
    /// Audit response model
    /// </summary>
    public class AuditResponse
    {
        /// <summary>
        /// Credentials risk report
        /// </summary>
        [JsonPropertyName("Credentials Risk Report")]
        public AuditReport? CredentialsRiskReport { get; set; }

        /// <summary>
        /// Database risk report
        /// </summary>
        [JsonPropertyName("Database Risk Report")]
        public AuditReport? DatabaseRiskReport { get; set; }

        /// <summary>
        /// Filesystem risk report
        /// </summary>
        [JsonPropertyName("Filesystem Risk Report")]
        public AuditReport? FilesystemRiskReport { get; set; }

        /// <summary>
        /// Nodes risk report
        /// </summary>
        [JsonPropertyName("Nodes Risk Report")]
        public AuditReport? NodesRiskReport { get; set; }

        /// <summary>
        /// Instance risk report
        /// </summary>
        [JsonPropertyName("Instance Risk Report")]
        public AuditReport? InstanceRiskReport { get; set; }
    }

    /// <summary>
    /// Audit report section
    /// </summary>
    public class AuditReport
    {
        /// <summary>
        /// Risk category
        /// </summary>
        [JsonPropertyName("risk")]
        public string? Risk { get; set; }

        /// <summary>
        /// Report sections
        /// </summary>
        [JsonPropertyName("sections")]
        public IList<AuditSection>? Sections { get; set; }
    }

    /// <summary>
    /// Audit report section
    /// </summary>
    public class AuditSection
    {
        /// <summary>
        /// Section title
        /// </summary>
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        /// <summary>
        /// Section description
        /// </summary>
        [JsonPropertyName("description")]
        public string? Description { get; set; }

        /// <summary>
        /// Recommendation
        /// </summary>
        [JsonPropertyName("recommendation")]
        public string? Recommendation { get; set; }

        /// <summary>
        /// Location information
        /// </summary>
        [JsonPropertyName("location")]
        public IList<AuditLocation>? Location { get; set; }
    }

    /// <summary>
    /// Audit location information
    /// </summary>
    public class AuditLocation
    {
        /// <summary>
        /// Location kind (credential, node, community, etc.)
        /// </summary>
        [JsonPropertyName("kind")]
        public string? Kind { get; set; }

        /// <summary>
        /// ID of the item
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Workflow ID (for nodes)
        /// </summary>
        [JsonPropertyName("workflowId")]
        public string? WorkflowId { get; set; }

        /// <summary>
        /// Workflow name (for nodes)
        /// </summary>
        [JsonPropertyName("workflowName")]
        public string? WorkflowName { get; set; }

        /// <summary>
        /// Node ID (for nodes)
        /// </summary>
        [JsonPropertyName("nodeId")]
        public string? NodeId { get; set; }

        /// <summary>
        /// Node name (for nodes)
        /// </summary>
        [JsonPropertyName("nodeName")]
        public string? NodeName { get; set; }

        /// <summary>
        /// Node type (for nodes)
        /// </summary>
        [JsonPropertyName("nodeType")]
        public string? NodeType { get; set; }

        /// <summary>
        /// Package URL (for community nodes)
        /// </summary>
        [JsonPropertyName("packageUrl")]
        public string? PackageUrl { get; set; }
    }

    /// <summary>
    /// Audit categories
    /// </summary>
    public static class AuditCategories
    {
        public const string Credentials = "credentials";
        public const string Database = "database";
        public const string Nodes = "nodes";
        public const string Filesystem = "filesystem";
        public const string Instance = "instance";
    }

    /// <summary>
    /// Source control pull request
    /// </summary>
    public class PullRequest
    {
        /// <summary>
        /// Whether to force the pull operation
        /// </summary>
        [JsonPropertyName("force")]
        public bool? Force { get; set; }

        /// <summary>
        /// Variables to set during the pull
        /// </summary>
        [JsonPropertyName("variables")]
        public Dictionary<string, object>? Variables { get; set; }
    }

    /// <summary>
    /// Import result from source control pull
    /// </summary>
    public class ImportResult
    {
        /// <summary>
        /// Variable import results
        /// </summary>
        [JsonPropertyName("variables")]
        public VariableImportResult? Variables { get; set; }

        /// <summary>
        /// Imported credentials
        /// </summary>
        [JsonPropertyName("credentials")]
        public IList<ImportedItem>? Credentials { get; set; }

        /// <summary>
        /// Imported workflows
        /// </summary>
        [JsonPropertyName("workflows")]
        public IList<ImportedItem>? Workflows { get; set; }

        /// <summary>
        /// Tag import results
        /// </summary>
        [JsonPropertyName("tags")]
        public TagImportResult? Tags { get; set; }
    }

    /// <summary>
    /// Variable import result
    /// </summary>
    public class VariableImportResult
    {
        /// <summary>
        /// Added variables
        /// </summary>
        [JsonPropertyName("added")]
        public IList<string>? Added { get; set; }

        /// <summary>
        /// Changed variables
        /// </summary>
        [JsonPropertyName("changed")]
        public IList<string>? Changed { get; set; }
    }

    /// <summary>
    /// Imported item information
    /// </summary>
    public class ImportedItem
    {
        /// <summary>
        /// Item ID
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Item name
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Item type (for credentials)
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    /// <summary>
    /// Tag import result
    /// </summary>
    public class TagImportResult
    {
        /// <summary>
        /// Imported tags
        /// </summary>
        [JsonPropertyName("tags")]
        public IList<ImportedItem>? Tags { get; set; }

        /// <summary>
        /// Tag mappings
        /// </summary>
        [JsonPropertyName("mappings")]
        public IList<TagMapping>? Mappings { get; set; }
    }

    /// <summary>
    /// Tag mapping information
    /// </summary>
    public class TagMapping
    {
        /// <summary>
        /// Workflow ID
        /// </summary>
        [JsonPropertyName("workflowId")]
        public string? WorkflowId { get; set; }

        /// <summary>
        /// Tag ID
        /// </summary>
        [JsonPropertyName("tagId")]
        public string? TagId { get; set; }
    }
}
