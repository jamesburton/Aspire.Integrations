using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flowise.Client.Models
{
    /// <summary>
    /// Response model for chatflow operations
    /// </summary>
    public class ChatflowResponse
    {
        /// <summary>
        /// Unique identifier for the chatflow
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// Name of the chatflow
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// JSON string containing the flow data/configuration
        /// </summary>
        [JsonPropertyName("flowData")]
        public string? FlowData { get; set; }

        /// <summary>
        /// Whether the chatflow is deployed and available
        /// </summary>
        [JsonPropertyName("deployed")]
        public bool Deployed { get; set; }

        /// <summary>
        /// Whether the chatflow is publicly accessible
        /// </summary>
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// API key ID associated with this chatflow (for access control)
        /// </summary>
        [JsonPropertyName("apikeyid")]
        public string? ApiKeyId { get; set; }

        /// <summary>
        /// JSON string containing chatbot configuration
        /// </summary>
        [JsonPropertyName("chatbotConfig")]
        public string? ChatbotConfig { get; set; }

        /// <summary>
        /// JSON string containing API configuration
        /// </summary>
        [JsonPropertyName("apiConfig")]
        public string? ApiConfig { get; set; }

        /// <summary>
        /// JSON string containing analytics configuration
        /// </summary>
        [JsonPropertyName("analytic")]
        public string? Analytic { get; set; }

        /// <summary>
        /// JSON string containing speech-to-text configuration
        /// </summary>
        [JsonPropertyName("speechToText")]
        public string? SpeechToText { get; set; }

        /// <summary>
        /// Semicolon-separated categories for organizing chatflows
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Type of the flow (CHATFLOW, AGENTFLOW, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// Date and time when the chatflow was created
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Date and time when the chatflow was last updated
        /// </summary>
        [JsonPropertyName("updatedDate")]
        public DateTime? UpdatedDate { get; set; }
    }

    /// <summary>
    /// Request model for creating a new chatflow
    /// </summary>
    public class CreateChatflowRequest
    {
        /// <summary>
        /// Name of the chatflow
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// JSON string containing the flow data/configuration
        /// </summary>
        [JsonPropertyName("flowData")]
        public string? FlowData { get; set; }

        /// <summary>
        /// Whether the chatflow should be deployed immediately
        /// </summary>
        [JsonPropertyName("deployed")]
        public bool Deployed { get; set; } = false;

        /// <summary>
        /// Whether the chatflow should be publicly accessible
        /// </summary>
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; } = true;

        /// <summary>
        /// API key ID to associate with this chatflow (for access control)
        /// </summary>
        [JsonPropertyName("apikeyid")]
        public string? ApiKeyId { get; set; }

        /// <summary>
        /// JSON string containing chatbot configuration
        /// </summary>
        [JsonPropertyName("chatbotConfig")]
        public string? ChatbotConfig { get; set; }

        /// <summary>
        /// JSON string containing API configuration
        /// </summary>
        [JsonPropertyName("apiConfig")]
        public string? ApiConfig { get; set; }

        /// <summary>
        /// JSON string containing analytics configuration
        /// </summary>
        [JsonPropertyName("analytic")]
        public string? Analytic { get; set; }

        /// <summary>
        /// JSON string containing speech-to-text configuration
        /// </summary>
        [JsonPropertyName("speechToText")]
        public string? SpeechToText { get; set; }

        /// <summary>
        /// Semicolon-separated categories for organizing chatflows
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Type of the flow (CHATFLOW, AGENTFLOW, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "CHATFLOW";
    }

    /// <summary>
    /// Request model for updating an existing chatflow
    /// </summary>
    public class UpdateChatflowRequest
    {
        /// <summary>
        /// Name of the chatflow
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// JSON string containing the flow data/configuration
        /// </summary>
        [JsonPropertyName("flowData")]
        public string? FlowData { get; set; }

        /// <summary>
        /// Whether the chatflow is deployed and available
        /// </summary>
        [JsonPropertyName("deployed")]
        public bool? Deployed { get; set; }

        /// <summary>
        /// Whether the chatflow is publicly accessible
        /// </summary>
        [JsonPropertyName("isPublic")]
        public bool? IsPublic { get; set; }

        /// <summary>
        /// API key ID to associate with this chatflow (for access control)
        /// </summary>
        [JsonPropertyName("apikeyid")]
        public string? ApiKeyId { get; set; }

        /// <summary>
        /// JSON string containing chatbot configuration
        /// </summary>
        [JsonPropertyName("chatbotConfig")]
        public string? ChatbotConfig { get; set; }

        /// <summary>
        /// JSON string containing API configuration
        /// </summary>
        [JsonPropertyName("apiConfig")]
        public string? ApiConfig { get; set; }

        /// <summary>
        /// JSON string containing analytics configuration
        /// </summary>
        [JsonPropertyName("analytic")]
        public string? Analytic { get; set; }

        /// <summary>
        /// JSON string containing speech-to-text configuration
        /// </summary>
        [JsonPropertyName("speechToText")]
        public string? SpeechToText { get; set; }

        /// <summary>
        /// Semicolon-separated categories for organizing chatflows
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Type of the flow (CHATFLOW, AGENTFLOW, etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    /// <summary>
    /// Enumeration of chatflow types
    /// </summary>
    public static class ChatflowTypes
    {
        public const string Chatflow = "CHATFLOW";
        public const string Agentflow = "AGENTFLOW";
        public const string Assistant = "ASSISTANT";
    }
}
