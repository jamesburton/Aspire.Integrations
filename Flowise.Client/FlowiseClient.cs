using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Flowise.Client.Models;

namespace Flowise.Client
{
    /// <summary>
    /// RestSharp-based client for Flowise AI API
    /// </summary>
    public class FlowiseClient : IFlowiseClient
    {
        private readonly RestClient _restClient;
        private readonly FlowiseClientOptions _options;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the FlowiseClient
        /// </summary>
        /// <param name="options">Configuration options for the client</param>
        public FlowiseClient(FlowiseClientOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            var clientOptions = new RestClientOptions(options.BaseUrl)
            {
                Timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds)
            };

            _restClient = new RestClient(clientOptions);

            // Add default headers if API key is provided
            if (!string.IsNullOrEmpty(options.ApiKey))
            {
                _restClient.AddDefaultHeader("Authorization", $"Bearer {options.ApiKey}");
            }

            _restClient.AddDefaultHeader("Accept", "*/*");
            //_restClient.AddDefaultHeader("Accept", "application/json");
            _restClient.AddDefaultHeader("Content-Type", "application/json");
        }

        #region Prediction API

        /// <summary>
        /// Creates a prediction (sends a message to a chatflow)
        /// </summary>
        /// <param name="request">The prediction request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Prediction response</returns>
        public async Task<PredictionResponse> CreatePredictionAsync(PredictionRequest request, CancellationToken cancellationToken = default)
        {
            ValidateRequest(request);

            var restRequest = new RestRequest($"api/v1/prediction/{request.ChatflowId}", Method.Post)
                .AddJsonBody(new
                {
                    question = request.Question,
                    streaming = request.Streaming,
                    overrideConfig = request.OverrideConfig,
                    history = request.History,
                    uploads = request.Uploads,
                    form = request.Form
                });

            var response = await _restClient.ExecuteAsync<PredictionResponse>(restRequest, cancellationToken);
            return HandleResponse(response);
        }

        /// <summary>
        /// Creates a streaming prediction
        /// </summary>
        /// <param name="request">The prediction request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Async enumerable of streaming chunks</returns>
        public async IAsyncEnumerable<string> CreateStreamingPredictionAsync(PredictionRequest request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            ValidateRequest(request);

            var streamingRequest = request with { Streaming = true };
            
            var restRequest = new RestRequest($"api/v1/prediction/{request.ChatflowId}", Method.Post)
                .AddJsonBody(new
                {
                    question = streamingRequest.Question,
                    streaming = true,
                    overrideConfig = streamingRequest.OverrideConfig,
                    history = streamingRequest.History,
                    uploads = streamingRequest.Uploads,
                    form = streamingRequest.Form
                });

            // For streaming, we need to handle the response differently
            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            
            if (!response.IsSuccessful)
            {
                throw new FlowiseClientException($"API request failed: {response.StatusCode} - {response.Content}");
            }

            // Split the streaming response into chunks
            if (!string.IsNullOrEmpty(response.Content))
            {
                var lines = response.Content.Split('\n');
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        yield return line;
                    }
                }
            }
        }

        #endregion

        #region Chatflows API

        /// <summary>
        /// Lists all chatflows
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of chatflows</returns>
        public async Task<IEnumerable<ChatflowResponse>> GetChatflowsAsync(CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("chatflows", Method.Get);
            var response = await _restClient.ExecuteAsync<IEnumerable<ChatflowResponse>>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <summary>
        /// Gets a specific chatflow by ID
        /// </summary>
        /// <param name="chatflowId">The chatflow ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Chatflow details</returns>
        public async Task<ChatflowResponse> GetChatflowAsync(string chatflowId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(chatflowId))
                throw new ArgumentException("Chatflow ID cannot be null or empty", nameof(chatflowId));

            var request = new RestRequest($"chatflows/{chatflowId}", Method.Get);
            var response = await _restClient.ExecuteAsync<ChatflowResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <summary>
        /// Gets a chatflow by API key
        /// </summary>
        /// <param name="apiKey">The API key</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Chatflow details</returns>
        public async Task<ChatflowResponse> GetChatflowByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException("API key cannot be null or empty", nameof(apiKey));

            var request = new RestRequest("chatflows/apikey")
                .AddParameter("apikey", apiKey);
            
            var response = await _restClient.ExecuteAsync<ChatflowResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <summary>
        /// Creates a new chatflow
        /// </summary>
        /// <param name="chatflow">Chatflow data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created chatflow</returns>
        public async Task<ChatflowResponse> CreateChatflowAsync(CreateChatflowRequest chatflow, CancellationToken cancellationToken = default)
        {
            if (chatflow == null)
                throw new ArgumentNullException(nameof(chatflow));

            var request = new RestRequest("chatflows", Method.Post)
                .AddJsonBody(chatflow);

            var response = await _restClient.ExecuteAsync<ChatflowResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <summary>
        /// Updates an existing chatflow
        /// </summary>
        /// <param name="chatflowId">The chatflow ID to update</param>
        /// <param name="chatflow">Updated chatflow data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated chatflow</returns>
        public async Task<ChatflowResponse> UpdateChatflowAsync(string chatflowId, UpdateChatflowRequest chatflow, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(chatflowId))
                throw new ArgumentException("Chatflow ID cannot be null or empty", nameof(chatflowId));
            
            if (chatflow == null)
                throw new ArgumentNullException(nameof(chatflow));

            var request = new RestRequest($"chatflows/{chatflowId}", Method.Put)
                .AddJsonBody(chatflow);

            var response = await _restClient.ExecuteAsync<ChatflowResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <summary>
        /// Deletes a chatflow
        /// </summary>
        /// <param name="chatflowId">The chatflow ID to delete</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the async operation</returns>
        public async Task DeleteChatflowAsync(string chatflowId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(chatflowId))
                throw new ArgumentException("Chatflow ID cannot be null or empty", nameof(chatflowId));

            var request = new RestRequest($"chatflows/{chatflowId}", Method.Delete);
            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            
            if (!response.IsSuccessful)
            {
                throw new FlowiseClientException($"Failed to delete chatflow: {response.StatusCode} - {response.Content}");
            }
        }

        #endregion

        #region Helper Methods

        private static void ValidateRequest(PredictionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            
            if (string.IsNullOrEmpty(request.ChatflowId))
                throw new ArgumentException("ChatflowId cannot be null or empty", nameof(request));
            
            if (string.IsNullOrEmpty(request.Question))
                throw new ArgumentException("Question cannot be null or empty", nameof(request));
        }

        private static T HandleResponse<T>(RestResponse<T> response)
        {
            if (!response.IsSuccessful)
            {
                throw new FlowiseClientException($"API request failed: {response.StatusCode} - {response.Content}");
            }

            return response.Data ?? throw new FlowiseClientException("Response data is null");
        }

        #endregion

        #region Ping

        /// <summary>
        /// Pings the Flowise server to check if it's alive
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if server is responding</returns>
        public async Task<bool> PingAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new RestRequest("ping", Method.Get);
                var response = await _restClient.ExecuteAsync(request, cancellationToken);
                return response.IsSuccessful;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _restClient?.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}
