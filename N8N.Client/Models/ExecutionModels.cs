using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace N8N.Client.Models
{
    /// <summary>
    /// Execution model
    /// </summary>
    public class Execution
    {
        /// <summary>
        /// Execution ID
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Execution data
        /// </summary>
        [JsonPropertyName("data")]
        public object? Data { get; set; }

        /// <summary>
        /// Whether the execution is finished
        /// </summary>
        [JsonPropertyName("finished")]
        public bool Finished { get; set; }

        /// <summary>
        /// Execution mode (cli, error, integrated, internal, manual, retry, trigger, webhook)
        /// </summary>
        [JsonPropertyName("mode")]
        public string? Mode { get; set; }

        /// <summary>
        /// ID of the execution this is a retry of (nullable)
        /// </summary>
        [JsonPropertyName("retryOf")]
        public int? RetryOf { get; set; }

        /// <summary>
        /// ID of the successful retry execution (nullable)
        /// </summary>
        [JsonPropertyName("retrySuccessId")]
        public int? RetrySuccessId { get; set; }

        /// <summary>
        /// Execution start time
        /// </summary>
        [JsonPropertyName("startedAt")]
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// Execution stop time (null for running executions)
        /// </summary>
        [JsonPropertyName("stoppedAt")]
        public DateTime? StoppedAt { get; set; }

        /// <summary>
        /// Workflow ID that was executed
        /// </summary>
        [JsonPropertyName("workflowId")]
        public int WorkflowId { get; set; }

        /// <summary>
        /// Time to wait until (for waiting executions)
        /// </summary>
        [JsonPropertyName("waitTill")]
        public DateTime? WaitTill { get; set; }

        /// <summary>
        /// Custom data for the execution
        /// </summary>
        [JsonPropertyName("customData")]
        public object? CustomData { get; set; }

        /// <summary>
        /// Execution status (canceled, crashed, error, new, running, success, unknown, waiting)
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    /// <summary>
    /// Execution list response
    /// </summary>
    public class ExecutionListResponse : PaginatedResponse<Execution>
    {
        /// <summary>
        /// The executions for this page
        /// </summary>
        [JsonPropertyName("data")]
        public new required IEnumerable<Execution> Data { get; set; }

        /// <summary>
        /// Cursor for the next page
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public new string? NextCursor { get; set; }
    }

    /// <summary>
    /// Options for listing executions
    /// </summary>
    public class ListExecutionsOptions : PaginationOptions
    {
        /// <summary>
        /// Whether to include execution data
        /// </summary>
        public bool? IncludeData { get; set; }

        /// <summary>
        /// Filter by execution status
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Filter by workflow ID
        /// </summary>
        public string? WorkflowId { get; set; }

        /// <summary>
        /// Filter by project ID
        /// </summary>
        public string? ProjectId { get; set; }
    }

    /// <summary>
    /// Options for getting a single execution
    /// </summary>
    public class GetExecutionOptions
    {
        /// <summary>
        /// Whether to include execution data
        /// </summary>
        public bool? IncludeData { get; set; }
    }

    /// <summary>
    /// Request for retrying an execution
    /// </summary>
    public class RetryExecutionRequest
    {
        /// <summary>
        /// Whether to load the currently saved workflow instead of the one saved at execution time
        /// </summary>
        [JsonPropertyName("loadWorkflow")]
        public bool? LoadWorkflow { get; set; }
    }

    /// <summary>
    /// Execution status filter values
    /// </summary>
    public static class ExecutionStatusFilter
    {
        public const string Canceled = "canceled";
        public const string Error = "error";
        public const string Running = "running";
        public const string Success = "success";
        public const string Waiting = "waiting";
    }
}
