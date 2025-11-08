using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flowise.Client.Models
{
    /// <summary>
    /// Request model for creating predictions
    /// </summary>
    public record PredictionRequest
    {
        /// <summary>
        /// The ID of the chatflow to use for prediction
        /// </summary>
        [JsonPropertyName("chatflowId")]
        public required string ChatflowId { get; init; }

        /// <summary>
        /// The question or message to send to the chatflow
        /// </summary>
        [JsonPropertyName("question")]
        public required string Question { get; init; }

        /// <summary>
        /// Whether to enable streaming responses
        /// </summary>
        [JsonPropertyName("streaming")]
        public bool Streaming { get; init; } = false;

        /// <summary>
        /// Configuration overrides for the chatflow
        /// </summary>
        [JsonPropertyName("overrideConfig")]
        public Dictionary<string, object>? OverrideConfig { get; init; }

        /// <summary>
        /// Conversation history for context
        /// </summary>
        [JsonPropertyName("history")]
        public List<ChatMessage>? History { get; init; }

        /// <summary>
        /// File uploads associated with the request
        /// </summary>
        [JsonPropertyName("uploads")]
        public List<UploadedFile>? Uploads { get; init; }

        /// <summary>
        /// Form data for the request
        /// </summary>
        [JsonPropertyName("form")]
        public Dictionary<string, object>? Form { get; init; }
    }

    /// <summary>
    /// Response model for predictions
    /// </summary>
    public class PredictionResponse
    {
        /// <summary>
        /// The response text from the AI
        /// </summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>
        /// The question that was asked
        /// </summary>
        [JsonPropertyName("question")]
        public string? Question { get; set; }

        /// <summary>
        /// The chatflow ID that was used
        /// </summary>
        [JsonPropertyName("chatId")]
        public string? ChatId { get; set; }

        /// <summary>
        /// Session ID for conversation tracking
        /// </summary>
        [JsonPropertyName("sessionId")]
        public string? SessionId { get; set; }

        /// <summary>
        /// Memory information for the conversation
        /// </summary>
        [JsonPropertyName("memoryType")]
        public string? MemoryType { get; set; }

        /// <summary>
        /// Source documents used (if RAG is enabled)
        /// </summary>
        [JsonPropertyName("sourceDocuments")]
        public List<SourceDocument>? SourceDocuments { get; set; }

        /// <summary>
        /// File annotations from the response
        /// </summary>
        [JsonPropertyName("fileAnnotations")]
        public List<FileAnnotation>? FileAnnotations { get; set; }

        /// <summary>
        /// Feedback information
        /// </summary>
        [JsonPropertyName("feedback")]
        public object? Feedback { get; set; }

        /// <summary>
        /// Additional metadata about the response
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// Represents a chat message in conversation history
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// The role of the message sender (user, assistant, system)
        /// </summary>
        [JsonPropertyName("role")]
        public required string Role { get; set; }

        /// <summary>
        /// The content of the message
        /// </summary>
        [JsonPropertyName("content")]
        public required string Content { get; set; }

        /// <summary>
        /// Timestamp when the message was created
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Additional metadata for the message
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }

    /// <summary>
    /// Represents an uploaded file
    /// </summary>
    public class UploadedFile
    {
        /// <summary>
        /// The name of the uploaded file
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        /// <summary>
        /// The MIME type of the file
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// The size of the file in bytes
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// Base64 encoded file data
        /// </summary>
        [JsonPropertyName("data")]
        public string? Data { get; set; }

        /// <summary>
        /// URL to the uploaded file (if stored remotely)
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    /// <summary>
    /// Represents a source document used in RAG responses
    /// </summary>
    public class SourceDocument
    {
        /// <summary>
        /// The content of the source document
        /// </summary>
        [JsonPropertyName("pageContent")]
        public string? PageContent { get; set; }

        /// <summary>
        /// Metadata about the source document
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }

        /// <summary>
        /// Relevance score for the document
        /// </summary>
        [JsonPropertyName("score")]
        public double? Score { get; set; }
    }

    /// <summary>
    /// Represents a file annotation in the response
    /// </summary>
    public class FileAnnotation
    {
        /// <summary>
        /// The ID of the file annotation
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// The type of annotation
        /// </summary>
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        /// <summary>
        /// The file path or URL
        /// </summary>
        [JsonPropertyName("filePath")]
        public string? FilePath { get; set; }

        /// <summary>
        /// MIME type of the annotated file
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string? MimeType { get; set; }
    }
}
