using System;
using System.Collections.Generic;

namespace Flowise.Client.Models
{
    /// <summary>
    /// Configuration options for the Flowise client
    /// </summary>
    public class FlowiseClientOptions
    {
        /// <summary>
        /// Base URL for the Flowise API (e.g., "http://localhost:3000" or "https://your-flowise-instance.com")
        /// </summary>
        public required string BaseUrl { get; set; }

        /// <summary>
        /// API key for authentication (Bearer token)
        /// </summary>
        public string? ApiKey { get; set; }

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
        public string UserAgent { get; set; } = "FlowiseClient/1.0";

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
    /// Exception thrown by Flowise client operations
    /// </summary>
    public class FlowiseClientException : Exception
    {
        /// <summary>
        /// HTTP status code associated with the error (if applicable)
        /// </summary>
        public int? StatusCode { get; }

        /// <summary>
        /// Response content from the server (if available)
        /// </summary>
        public string? ResponseContent { get; }

        public FlowiseClientException(string message) : base(message)
        {
        }

        public FlowiseClientException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FlowiseClientException(string message, int statusCode, string? responseContent = null) : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }

        public FlowiseClientException(string message, int statusCode, string? responseContent, Exception innerException) 
            : base(message, innerException)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }

    /// <summary>
    /// Helper class for creating common override configuration objects
    /// </summary>
    public static class OverrideConfigBuilder
    {
        /// <summary>
        /// Creates an override config with session ID
        /// </summary>
        /// <param name="sessionId">Session ID for conversation tracking</param>
        /// <returns>Override configuration dictionary</returns>
        public static Dictionary<string, object> WithSessionId(string sessionId)
        {
            return new Dictionary<string, object>
            {
                ["sessionId"] = sessionId
            };
        }

        /// <summary>
        /// Creates an override config with temperature setting
        /// </summary>
        /// <param name="temperature">Temperature value (0.0 to 1.0)</param>
        /// <returns>Override configuration dictionary</returns>
        public static Dictionary<string, object> WithTemperature(double temperature)
        {
            return new Dictionary<string, object>
            {
                ["temperature"] = temperature
            };
        }

        /// <summary>
        /// Creates an override config with max tokens setting
        /// </summary>
        /// <param name="maxTokens">Maximum number of tokens</param>
        /// <returns>Override configuration dictionary</returns>
        public static Dictionary<string, object> WithMaxTokens(int maxTokens)
        {
            return new Dictionary<string, object>
            {
                ["maxTokens"] = maxTokens
            };
        }

        /// <summary>
        /// Creates an override config with return source documents setting
        /// </summary>
        /// <param name="returnSourceDocuments">Whether to return source documents</param>
        /// <returns>Override configuration dictionary</returns>
        public static Dictionary<string, object> WithReturnSourceDocuments(bool returnSourceDocuments = true)
        {
            return new Dictionary<string, object>
            {
                ["returnSourceDocuments"] = returnSourceDocuments
            };
        }

        /// <summary>
        /// Creates a comprehensive override config
        /// </summary>
        /// <param name="sessionId">Session ID for conversation tracking</param>
        /// <param name="temperature">Temperature value (0.0 to 1.0)</param>
        /// <param name="maxTokens">Maximum number of tokens</param>
        /// <param name="returnSourceDocuments">Whether to return source documents</param>
        /// <returns>Override configuration dictionary</returns>
        public static Dictionary<string, object> Create(
            string? sessionId = null,
            double? temperature = null,
            int? maxTokens = null,
            bool? returnSourceDocuments = null)
        {
            var config = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(sessionId))
                config["sessionId"] = sessionId;

            if (temperature.HasValue)
                config["temperature"] = temperature.Value;

            if (maxTokens.HasValue)
                config["maxTokens"] = maxTokens.Value;

            if (returnSourceDocuments.HasValue)
                config["returnSourceDocuments"] = returnSourceDocuments.Value;

            return config;
        }
    }
}
