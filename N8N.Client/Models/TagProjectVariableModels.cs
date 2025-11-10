using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace N8N.Client.Models
{
    /// <summary>
    /// Tag model
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Tag ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Tag name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

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
    /// Tag list response
    /// </summary>
    public class TagListResponse : PaginatedResponse<Tag>
    {
        /// <summary>
        /// The tags for this page
        /// </summary>
        [JsonPropertyName("data")]
        public new required IEnumerable<Tag> Data { get; set; }

        /// <summary>
        /// Cursor for the next page
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public new string? NextCursor { get; set; }
    }

    /// <summary>
    /// Project model
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Project ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Project name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Project type (read-only)
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    /// <summary>
    /// Project list response
    /// </summary>
    public class ProjectListResponse : PaginatedResponse<Project>
    {
        /// <summary>
        /// The projects for this page
        /// </summary>
        [JsonPropertyName("data")]
        public new required IEnumerable<Project> Data { get; set; }

        /// <summary>
        /// Cursor for the next page
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public new string? NextCursor { get; set; }
    }

    /// <summary>
    /// Request for adding users to a project
    /// </summary>
    public class AddUsersToProjectRequest
    {
        /// <summary>
        /// List of user-role relations to add to the project
        /// </summary>
        [JsonPropertyName("relations")]
        public required IList<ProjectUserRelation> Relations { get; set; }
    }

    /// <summary>
    /// Project user relation
    /// </summary>
    public class ProjectUserRelation
    {
        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("userId")]
        public required string UserId { get; set; }

        /// <summary>
        /// Role in the project
        /// </summary>
        [JsonPropertyName("role")]
        public required string Role { get; set; }
    }

    /// <summary>
    /// Request for changing a user's role in a project
    /// </summary>
    public class ChangeProjectUserRoleRequest
    {
        /// <summary>
        /// New role for the user in the project
        /// </summary>
        [JsonPropertyName("role")]
        public required string Role { get; set; }
    }

    /// <summary>
    /// Variable model
    /// </summary>
    public class Variable
    {
        /// <summary>
        /// Variable ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Variable key
        /// </summary>
        [JsonPropertyName("key")]
        public required string Key { get; set; }

        /// <summary>
        /// Variable value
        /// </summary>
        [JsonPropertyName("value")]
        public required string Value { get; set; }

        /// <summary>
        /// Variable type (read-only)
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Associated project
        /// </summary>
        [JsonPropertyName("project")]
        public Project? Project { get; set; }
    }

    /// <summary>
    /// Variable create/update request
    /// </summary>
    public class VariableCreateRequest
    {
        /// <summary>
        /// Variable key
        /// </summary>
        [JsonPropertyName("key")]
        public required string Key { get; set; }

        /// <summary>
        /// Variable value
        /// </summary>
        [JsonPropertyName("value")]
        public required string Value { get; set; }

        /// <summary>
        /// Project ID (nullable)
        /// </summary>
        [JsonPropertyName("projectId")]
        public string? ProjectId { get; set; }
    }

    /// <summary>
    /// Variable list response
    /// </summary>
    public class VariableListResponse : PaginatedResponse<Variable>
    {
        /// <summary>
        /// The variables for this page
        /// </summary>
        [JsonPropertyName("data")]
        public new required IEnumerable<Variable> Data { get; set; }

        /// <summary>
        /// Cursor for the next page
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public new string? NextCursor { get; set; }
    }

    /// <summary>
    /// Options for listing variables
    /// </summary>
    public class ListVariablesOptions : PaginationOptions
    {
        /// <summary>
        /// Filter by project ID
        /// </summary>
        public string? ProjectId { get; set; }

        /// <summary>
        /// Filter by state (empty)
        /// </summary>
        public string? State { get; set; }
    }

    /// <summary>
    /// Variable state filter values
    /// </summary>
    public static class VariableState
    {
        public const string Empty = "empty";
    }
}
