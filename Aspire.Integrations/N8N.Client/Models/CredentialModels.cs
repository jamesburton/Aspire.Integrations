using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace N8N.Client.Models
{
    /// <summary>
    /// Credential model
    /// </summary>
    public class Credential
    {
        /// <summary>
        /// Credential ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Credential name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Credential type (e.g., "github")
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// Credential data (write-only)
        /// </summary>
        [JsonPropertyName("data")]
        public required Dictionary<string, object> Data { get; set; }

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
    /// Response when creating a credential
    /// </summary>
    public class CreateCredentialResponse
    {
        /// <summary>
        /// Credential ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }

        /// <summary>
        /// Credential name
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// Credential type
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// Creation timestamp (read-only)
        /// </summary>
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (read-only)
        /// </summary>
        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Request for transferring a credential to another project
    /// </summary>
    public class TransferCredentialRequest
    {
        /// <summary>
        /// The ID of the destination project
        /// </summary>
        [JsonPropertyName("destinationProjectId")]
        public required string DestinationProjectId { get; set; }
    }

    /// <summary>
    /// Credential type schema response
    /// </summary>
    public class CredentialTypeSchema
    {
        /// <summary>
        /// Whether additional properties are allowed
        /// </summary>
        [JsonPropertyName("additionalProperties")]
        public bool AdditionalProperties { get; set; }

        /// <summary>
        /// Schema type
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Schema properties
        /// </summary>
        [JsonPropertyName("properties")]
        public Dictionary<string, object>? Properties { get; set; }

        /// <summary>
        /// Required fields
        /// </summary>
        [JsonPropertyName("required")]
        public IList<string>? Required { get; set; }
    }

    /// <summary>
    /// Credential type information
    /// </summary>
    public class CredentialType
    {
        /// <summary>
        /// Display name of the credential type
        /// </summary>
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Name of the credential type
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Type of the field
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Default value
        /// </summary>
        [JsonPropertyName("default")]
        public string? Default { get; set; }
    }
}
