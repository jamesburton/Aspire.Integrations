using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flowise.Client.Models;

namespace Flowise.Client
{
    /// <summary>
    /// Interface for the Flowise AI API client
    /// </summary>
    public interface IFlowiseClient : IDisposable
    {
        #region Prediction API

        /// <summary>
        /// Creates a prediction (sends a message to a chatflow)
        /// </summary>
        /// <param name="request">The prediction request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Prediction response</returns>
        Task<PredictionResponse> CreatePredictionAsync(PredictionRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a streaming prediction
        /// </summary>
        /// <param name="request">The prediction request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Async enumerable of streaming chunks</returns>
        IAsyncEnumerable<string> CreateStreamingPredictionAsync(PredictionRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Chatflows API

        /// <summary>
        /// Lists all chatflows
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of chatflows</returns>
        Task<IEnumerable<ChatflowResponse>> GetChatflowsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a specific chatflow by ID
        /// </summary>
        /// <param name="chatflowId">The chatflow ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Chatflow details</returns>
        Task<ChatflowResponse> GetChatflowAsync(string chatflowId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a chatflow by API key
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Chatflow details</returns>
        Task<ChatflowResponse> GetChatflowByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new chatflow
        /// </summary>
        /// <param name="chatflow">Chatflow data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created chatflow</returns>
        Task<ChatflowResponse> CreateChatflowAsync(CreateChatflowRequest chatflow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing chatflow
        /// </summary>
        /// <param name="chatflowId">The chatflow ID to update</param>
        /// <param name="chatflow">Updated chatflow data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated chatflow</returns>
        Task<ChatflowResponse> UpdateChatflowAsync(string chatflowId, UpdateChatflowRequest chatflow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a chatflow
        /// </summary>
        /// <param name="chatflowId">The chatflow ID to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        Task DeleteChatflowAsync(string chatflowId, CancellationToken cancellationToken = default);

        #endregion

        #region Utility

        /// <summary>
        /// Pings the Flowise server to check if it's alive
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if server is responding</returns>
        Task<bool> PingAsync(CancellationToken cancellationToken = default);

        #endregion
    }
}
