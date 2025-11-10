using System;
using System.Collections.Generic;

namespace N8N.Client.Models
{
    /// <summary>
    /// Configuration options for the N8N client
    /// </summary>
    public class N8NClientOptions
    {
        /// <summary>
        /// Base URL for the N8N API (e.g., "http://localhost:5678" or "https://your-n8n-instance.com")
        /// </summary>
        public required string BaseUrl { get; set; }

        /// <summary>
        /// API key for authentication (sent via X-N8N-API-KEY header)
        /// </summary>
        public required string ApiKey { get; set; }

        /// <summary>
        /// Timeout for HTTP requests in milliseconds
        /// </summary>
        public int TimeoutMilliseconds { get; set; } = 30000; // 30 seconds default

        /// <summary>
        /// Whether to validate SSL certificates (set to false for development with self-signed certificates)
        /// </summary>
        public bool ValidateSslCertificates { get; set; } = true;

        /// <summary>
        /// User agent string to send with requests
        /// </summary>
        public string UserAgent { get; set; } = "N8NClient/1.0";

        /// <summary>
        /// Maximum number of retry attempts for failed requests
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Delay between retry attempts in milliseconds
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }

    /// <summary>
    /// Exception thrown by N8N client operations
    /// </summary>
    public class N8NClientException : Exception
    {
        /// <summary>
        /// HTTP status code associated with the error (if applicable)
        /// </summary>
        public int? StatusCode { get; }

        /// <summary>
        /// Response content from the server (if available)
        /// </summary>
        public string? ResponseContent { get; }

        /// <summary>
        /// N8N error code (if available)
        /// </summary>
        public string? ErrorCode { get; }

        public N8NClientException(string message) : base(message)
        {
        }

        public N8NClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public N8NClientException(string message, int statusCode, string? responseContent = null, string? errorCode = null) : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
            ErrorCode = errorCode;
        }

        public N8NClientException(string message, int statusCode, string? responseContent, string? errorCode, Exception innerException) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
            ErrorCode = errorCode;
        }
    }

    /// <summary>
    /// Helper class for pagination parameters
    /// </summary>
    public class PaginationOptions
    {
        /// <summary>
        /// The maximum number of items to return (max 250, default 100)
        /// </summary>
        public int? Limit { get; set; } = 100;

        /// <summary>
        /// Cursor for pagination (from previous response's nextCursor)
        /// </summary>
        public string? Cursor { get; set; }
    }

    /// <summary>
    /// Paginated response wrapper
    /// </summary>
    /// <typeparam name="T">Type of data items</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// The data items for this page
        /// </summary>
        public IEnumerable<T> Data { get; set; } = [];

        /// <summary>
        /// Cursor for the next page (null if this is the last page)
        /// </summary>
        public string? NextCursor { get; set; }
    }

    /// <summary>
    /// Common error response from N8N API
    /// </summary>
    public class N8NError
    {
        /// <summary>
        /// Error code
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Error description
        /// </summary>
        public string? Description { get; set; }
    }

    /// <summary>
    /// N8N execution status enumeration
    /// </summary>
    public static class ExecutionStatus
    {
        public const string Canceled = "canceled";
        public const string Crashed = "crashed";
        public const string Error = "error";
        public const string New = "new";
        public const string Running = "running";
        public const string Success = "success";
        public const string Unknown = "unknown";
        public const string Waiting = "waiting";
    }

    /// <summary>
    /// N8N execution mode enumeration
    /// </summary>
    public static class ExecutionMode
    {
        public const string Cli = "cli";
        public const string Error = "error";
        public const string Integrated = "integrated";
        public const string Internal = "internal";
        public const string Manual = "manual";
        public const string Retry = "retry";
        public const string Trigger = "trigger";
        public const string Webhook = "webhook";
    }

    /// <summary>
    /// N8N user roles
    /// </summary>
    public static class UserRoles
    {
        public const string GlobalOwner = "global:owner";
        public const string GlobalAdmin = "global:admin";
        public const string GlobalMember = "global:member";
        public const string ProjectAdmin = "project:admin";
        public const string ProjectEditor = "project:editor";
        public const string ProjectViewer = "project:viewer";
    }

    /// <summary>
    /// Workflow settings data save options
    /// </summary>
    public static class DataSaveOptions
    {
        public const string All = "all";
        public const string None = "none";
    }
}
