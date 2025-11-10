using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using N8N.Client.Models;

namespace N8N.Client
{
    /// <summary>
    /// RestSharp-based client for N8N API
    /// </summary>
    public class N8NClient : IN8NClient
    {
        private readonly RestClient _restClient;
        private readonly N8NClientOptions _options;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the N8NClient
        /// </summary>
        /// <param name="options">Configuration options for the client</param>
        public N8NClient(N8NClientOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            var clientOptions = new RestClientOptions(options.BaseUrl.TrimEnd('/'))
            {
                Timeout = TimeSpan.FromMilliseconds(options.TimeoutMilliseconds),
                UserAgent = options.UserAgent
            };

            _restClient = new RestClient(clientOptions);

            // Add API key header for authentication
            _restClient.AddDefaultHeader("X-N8N-API-KEY", options.ApiKey);
        }

        #region Workflow API

        /// <inheritdoc/>
        public async Task<WorkflowListResponse> GetWorkflowsAsync(ListWorkflowsOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("api/v1/workflows", Method.Get);
            
            if (options != null)
            {
                if (options.Active.HasValue)
                    request.AddQueryParameter("active", options.Active.Value);
                if (!string.IsNullOrEmpty(options.Tags))
                    request.AddQueryParameter("tags", options.Tags);
                if (!string.IsNullOrEmpty(options.Name))
                    request.AddQueryParameter("name", options.Name);
                if (!string.IsNullOrEmpty(options.ProjectId))
                    request.AddQueryParameter("projectId", options.ProjectId);
                if (options.ExcludePinnedData.HasValue)
                    request.AddQueryParameter("excludePinnedData", options.ExcludePinnedData.Value);
                if (options.Limit.HasValue)
                    request.AddQueryParameter("limit", options.Limit.Value);
                if (!string.IsNullOrEmpty(options.Cursor))
                    request.AddQueryParameter("cursor", options.Cursor);
            }

            var response = await _restClient.ExecuteAsync<WorkflowListResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Workflow> GetWorkflowAsync(string workflowId, GetWorkflowOptions? options = null, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);

            var request = new RestRequest($"api/v1/workflows/{workflowId}", Method.Get);
            
            if (options?.ExcludePinnedData == true)
                request.AddQueryParameter("excludePinnedData", true);

            var response = await _restClient.ExecuteAsync<Workflow>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Workflow> CreateWorkflowAsync(CreateWorkflowRequest workflow, CancellationToken cancellationToken = default)
        {
            if (workflow == null)
                throw new ArgumentNullException(nameof(workflow));

            var request = new RestRequest("api/v1/workflows", Method.Post)
                .AddJsonBody(workflow);

            var response = await _restClient.ExecuteAsync<Workflow>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Workflow> UpdateWorkflowAsync(string workflowId, UpdateWorkflowRequest workflow, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);
            if (workflow == null)
                throw new ArgumentNullException(nameof(workflow));

            var request = new RestRequest($"api/v1/workflows/{workflowId}", Method.Put)
                .AddJsonBody(workflow);

            var response = await _restClient.ExecuteAsync<Workflow>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Workflow> DeleteWorkflowAsync(string workflowId, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);

            var request = new RestRequest($"api/v1/workflows/{workflowId}", Method.Delete);
            var response = await _restClient.ExecuteAsync<Workflow>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Workflow> ActivateWorkflowAsync(string workflowId, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);

            var request = new RestRequest($"api/v1/workflows/{workflowId}/activate", Method.Post);
            var response = await _restClient.ExecuteAsync<Workflow>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Workflow> DeactivateWorkflowAsync(string workflowId, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);

            var request = new RestRequest($"api/v1/workflows/{workflowId}/deactivate", Method.Post);
            var response = await _restClient.ExecuteAsync<Workflow>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task TransferWorkflowAsync(string workflowId, TransferWorkflowRequest request, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest($"api/v1/workflows/{workflowId}/transfer", Method.Put)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Tag>> GetWorkflowTagsAsync(string workflowId, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);

            var request = new RestRequest($"api/v1/workflows/{workflowId}/tags", Method.Get);
            var response = await _restClient.ExecuteAsync<IEnumerable<Tag>>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Tag>> UpdateWorkflowTagsAsync(string workflowId, UpdateWorkflowTagsRequest request, CancellationToken cancellationToken = default)
        {
            ValidateWorkflowId(workflowId);
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest($"api/v1/workflows/{workflowId}/tags", Method.Put)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync<IEnumerable<Tag>>(restRequest, cancellationToken);
            return HandleResponse(response);
        }

        #endregion

        #region Execution API

        /// <inheritdoc/>
        public async Task<ExecutionListResponse> GetExecutionsAsync(ListExecutionsOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("api/v1/executions", Method.Get);
            
            if (options != null)
            {
                if (options.IncludeData.HasValue)
                    request.AddQueryParameter("includeData", options.IncludeData.Value);
                if (!string.IsNullOrEmpty(options.Status))
                    request.AddQueryParameter("status", options.Status);
                if (!string.IsNullOrEmpty(options.WorkflowId))
                    request.AddQueryParameter("workflowId", options.WorkflowId);
                if (!string.IsNullOrEmpty(options.ProjectId))
                    request.AddQueryParameter("projectId", options.ProjectId);
                if (options.Limit.HasValue)
                    request.AddQueryParameter("limit", options.Limit.Value);
                if (!string.IsNullOrEmpty(options.Cursor))
                    request.AddQueryParameter("cursor", options.Cursor);
            }

            var response = await _restClient.ExecuteAsync<ExecutionListResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Execution> GetExecutionAsync(int executionId, GetExecutionOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest($"api/v1/executions/{executionId}", Method.Get);
            
            if (options?.IncludeData == true)
                request.AddQueryParameter("includeData", true);

            var response = await _restClient.ExecuteAsync<Execution>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Execution> DeleteExecutionAsync(int executionId, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest($"api/v1/executions/{executionId}", Method.Delete);
            var response = await _restClient.ExecuteAsync<Execution>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Execution> RetryExecutionAsync(int executionId, RetryExecutionRequest? request = null, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest($"api/v1/executions/{executionId}/retry", Method.Post);
            
            if (request != null)
                restRequest.AddJsonBody(request);

            var response = await _restClient.ExecuteAsync<Execution>(restRequest, cancellationToken);
            return HandleResponse(response);
        }

        #endregion

        #region Credential API

        /// <inheritdoc/>
        public async Task<CreateCredentialResponse> CreateCredentialAsync(Credential credential, CancellationToken cancellationToken = default)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            var request = new RestRequest("api/v1/credentials", Method.Post)
                .AddJsonBody(credential);

            var response = await _restClient.ExecuteAsync<CreateCredentialResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Credential> DeleteCredentialAsync(string credentialId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentException("Credential ID cannot be null or empty", nameof(credentialId));

            var request = new RestRequest($"api/v1/credentials/{credentialId}", Method.Delete);
            var response = await _restClient.ExecuteAsync<Credential>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<CredentialTypeSchema> GetCredentialTypeSchemaAsync(string credentialTypeName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(credentialTypeName))
                throw new ArgumentException("Credential type name cannot be null or empty", nameof(credentialTypeName));

            var request = new RestRequest($"api/v1/credentials/schema/{credentialTypeName}", Method.Get);
            var response = await _restClient.ExecuteAsync<CredentialTypeSchema>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task TransferCredentialAsync(string credentialId, TransferCredentialRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(credentialId))
                throw new ArgumentException("Credential ID cannot be null or empty", nameof(credentialId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest($"api/v1/credentials/{credentialId}/transfer", Method.Put)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            HandleVoidResponse(response);
        }

        #endregion

        #region Tag API

        /// <inheritdoc/>
        public async Task<TagListResponse> GetTagsAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("api/v1/tags", Method.Get);
            
            if (options != null)
            {
                if (options.Limit.HasValue)
                    request.AddQueryParameter("limit", options.Limit.Value);
                if (!string.IsNullOrEmpty(options.Cursor))
                    request.AddQueryParameter("cursor", options.Cursor);
            }

            var response = await _restClient.ExecuteAsync<TagListResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Tag> GetTagAsync(string tagId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentException("Tag ID cannot be null or empty", nameof(tagId));

            var request = new RestRequest($"api/v1/tags/{tagId}", Method.Get);
            var response = await _restClient.ExecuteAsync<Tag>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Tag> CreateTagAsync(Tag tag, CancellationToken cancellationToken = default)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            var request = new RestRequest("api/v1/tags", Method.Post)
                .AddJsonBody(tag);

            var response = await _restClient.ExecuteAsync<Tag>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Tag> UpdateTagAsync(string tagId, Tag tag, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentException("Tag ID cannot be null or empty", nameof(tagId));
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            var request = new RestRequest($"api/v1/tags/{tagId}", Method.Put)
                .AddJsonBody(tag);

            var response = await _restClient.ExecuteAsync<Tag>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<Tag> DeleteTagAsync(string tagId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(tagId))
                throw new ArgumentException("Tag ID cannot be null or empty", nameof(tagId));

            var request = new RestRequest($"api/v1/tags/{tagId}", Method.Delete);
            var response = await _restClient.ExecuteAsync<Tag>(request, cancellationToken);
            return HandleResponse(response);
        }

        #endregion

        #region User API (Enterprise features)

        /// <inheritdoc/>
        public async Task<UserListResponse> GetUsersAsync(ListUsersOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("api/v1/users", Method.Get);
            
            if (options != null)
            {
                if (options.Limit.HasValue)
                    request.AddQueryParameter("limit", options.Limit.Value);
                if (!string.IsNullOrEmpty(options.Cursor))
                    request.AddQueryParameter("cursor", options.Cursor);
                if (options.IncludeRole.HasValue)
                    request.AddQueryParameter("includeRole", options.IncludeRole.Value);
                if (!string.IsNullOrEmpty(options.ProjectId))
                    request.AddQueryParameter("projectId", options.ProjectId);
            }

            var response = await _restClient.ExecuteAsync<UserListResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<User> GetUserAsync(string userIdentifier, GetUserOptions? options = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userIdentifier))
                throw new ArgumentException("User identifier cannot be null or empty", nameof(userIdentifier));

            var request = new RestRequest($"api/v1/users/{userIdentifier}", Method.Get);
            
            if (options?.IncludeRole == true)
                request.AddQueryParameter("includeRole", true);

            var response = await _restClient.ExecuteAsync<User>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CreateUserResponse>> CreateUsersAsync(IEnumerable<CreateUserRequest> users, CancellationToken cancellationToken = default)
        {
            if (users == null)
                throw new ArgumentNullException(nameof(users));

            var usersList = users.ToList();
            if (!usersList.Any())
                throw new ArgumentException("Users collection cannot be empty", nameof(users));

            var request = new RestRequest("api/v1/users", Method.Post)
                .AddJsonBody(usersList);

            var response = await _restClient.ExecuteAsync<IEnumerable<CreateUserResponse>>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task DeleteUserAsync(string userIdentifier, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userIdentifier))
                throw new ArgumentException("User identifier cannot be null or empty", nameof(userIdentifier));

            var request = new RestRequest($"api/v1/users/{userIdentifier}", Method.Delete);
            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task ChangeUserRoleAsync(string userIdentifier, ChangeUserRoleRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userIdentifier))
                throw new ArgumentException("User identifier cannot be null or empty", nameof(userIdentifier));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest($"api/v1/users/{userIdentifier}/role", Method.Patch)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            HandleVoidResponse(response);
        }

        #endregion

        #region Variable API

        /// <inheritdoc/>
        public async Task<VariableListResponse> GetVariablesAsync(ListVariablesOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("api/v1/variables", Method.Get);
            
            if (options != null)
            {
                if (options.Limit.HasValue)
                    request.AddQueryParameter("limit", options.Limit.Value);
                if (!string.IsNullOrEmpty(options.Cursor))
                    request.AddQueryParameter("cursor", options.Cursor);
                if (!string.IsNullOrEmpty(options.ProjectId))
                    request.AddQueryParameter("projectId", options.ProjectId);
                if (!string.IsNullOrEmpty(options.State))
                    request.AddQueryParameter("state", options.State);
            }

            var response = await _restClient.ExecuteAsync<VariableListResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task CreateVariableAsync(VariableCreateRequest variable, CancellationToken cancellationToken = default)
        {
            if (variable == null)
                throw new ArgumentNullException(nameof(variable));

            var request = new RestRequest("api/v1/variables", Method.Post)
                .AddJsonBody(variable);

            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task UpdateVariableAsync(string variableId, VariableCreateRequest variable, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(variableId))
                throw new ArgumentException("Variable ID cannot be null or empty", nameof(variableId));
            if (variable == null)
                throw new ArgumentNullException(nameof(variable));

            var request = new RestRequest($"api/v1/variables/{variableId}", Method.Put)
                .AddJsonBody(variable);

            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task DeleteVariableAsync(string variableId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(variableId))
                throw new ArgumentException("Variable ID cannot be null or empty", nameof(variableId));

            var request = new RestRequest($"api/v1/variables/{variableId}", Method.Delete);
            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        #endregion

        #region Project API

        /// <inheritdoc/>
        public async Task<ProjectListResponse> GetProjectsAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default)
        {
            var request = new RestRequest("api/v1/projects", Method.Get);
            
            if (options != null)
            {
                if (options.Limit.HasValue)
                    request.AddQueryParameter("limit", options.Limit.Value);
                if (!string.IsNullOrEmpty(options.Cursor))
                    request.AddQueryParameter("cursor", options.Cursor);
            }

            var response = await _restClient.ExecuteAsync<ProjectListResponse>(request, cancellationToken);
            return HandleResponse(response);
        }

        /// <inheritdoc/>
        public async Task CreateProjectAsync(Project project, CancellationToken cancellationToken = default)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var request = new RestRequest("api/v1/projects", Method.Post)
                .AddJsonBody(project);

            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task UpdateProjectAsync(string projectId, Project project, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            var request = new RestRequest($"api/v1/projects/{projectId}", Method.Put)
                .AddJsonBody(project);

            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));

            var request = new RestRequest($"api/v1/projects/{projectId}", Method.Delete);
            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task AddUsersToProjectAsync(string projectId, AddUsersToProjectRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest($"api/v1/projects/{projectId}/users", Method.Post)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task RemoveUserFromProjectAsync(string projectId, string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            var request = new RestRequest($"api/v1/projects/{projectId}/users/{userId}", Method.Delete);
            var response = await _restClient.ExecuteAsync(request, cancellationToken);
            HandleVoidResponse(response);
        }

        /// <inheritdoc/>
        public async Task ChangeUserRoleInProjectAsync(string projectId, string userId, ChangeProjectUserRoleRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(projectId))
                throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest($"api/v1/projects/{projectId}/users/{userId}", Method.Patch)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync(restRequest, cancellationToken);
            HandleVoidResponse(response);
        }

        #endregion

        #region Audit API

        /// <inheritdoc/>
        public async Task<AuditResponse> GenerateAuditAsync(GenerateAuditRequest? request = null, CancellationToken cancellationToken = default)
        {
            var restRequest = new RestRequest("api/v1/audit", Method.Post);
            
            if (request != null)
                restRequest.AddJsonBody(request);

            var response = await _restClient.ExecuteAsync<AuditResponse>(restRequest, cancellationToken);
            return HandleResponse(response);
        }

        #endregion

        #region Source Control API

        /// <inheritdoc/>
        public async Task<ImportResult> PullFromRepositoryAsync(PullRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var restRequest = new RestRequest("api/v1/source-control/pull", Method.Post)
                .AddJsonBody(request);

            var response = await _restClient.ExecuteAsync<ImportResult>(restRequest, cancellationToken);
            return HandleResponse(response);
        }

        #endregion

        #region Helper Methods

        private static void ValidateWorkflowId(string workflowId)
        {
            if (string.IsNullOrEmpty(workflowId))
                throw new ArgumentException("Workflow ID cannot be null or empty", nameof(workflowId));
        }

        private static T HandleResponse<T>(RestResponse<T> response)
        {
            if (!response.IsSuccessful)
            {
                var errorMessage = $"N8N API request failed: {response.StatusCode}";
                if (!string.IsNullOrEmpty(response.Content))
                {
                    errorMessage += $" - {response.Content}";
                }

                throw new N8NClientException(errorMessage, (int)response.StatusCode, response.Content);
            }

            return response.Data ?? throw new N8NClientException("Response data is null");
        }

        private static void HandleVoidResponse(RestResponse response)
        {
            if (!response.IsSuccessful)
            {
                var errorMessage = $"N8N API request failed: {response.StatusCode}";
                if (!string.IsNullOrEmpty(response.Content))
                {
                    errorMessage += $" - {response.Content}";
                }

                throw new N8NClientException(errorMessage, (int)response.StatusCode, response.Content);
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
