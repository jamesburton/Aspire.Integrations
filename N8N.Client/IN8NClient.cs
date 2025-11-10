using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using N8N.Client.Models;

namespace N8N.Client
{
    /// <summary>
    /// Interface for the N8N API client
    /// </summary>
    public interface IN8NClient : IDisposable
    {
        #region Workflow API

        /// <summary>
        /// Retrieve all workflows
        /// </summary>
        /// <param name="options">Options for filtering workflows</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of workflows</returns>
        Task<WorkflowListResponse> GetWorkflowsAsync(ListWorkflowsOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a specific workflow
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="options">Options for getting the workflow</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Workflow details</returns>
        Task<Workflow> GetWorkflowAsync(string workflowId, GetWorkflowOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new workflow
        /// </summary>
        /// <param name="workflow">Workflow to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created workflow</returns>
        Task<Workflow> CreateWorkflowAsync(CreateWorkflowRequest workflow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing workflow
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="workflow">Updated workflow data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated workflow</returns>
        Task<Workflow> UpdateWorkflowAsync(string workflowId, UpdateWorkflowRequest workflow, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a workflow
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deleted workflow</returns>
        Task<Workflow> DeleteWorkflowAsync(string workflowId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Activate a workflow
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated workflow</returns>
        Task<Workflow> ActivateWorkflowAsync(string workflowId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivate a workflow
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated workflow</returns>
        Task<Workflow> DeactivateWorkflowAsync(string workflowId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Transfer a workflow to another project
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="request">Transfer request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task TransferWorkflowAsync(string workflowId, TransferWorkflowRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get workflow tags
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of tags</returns>
        Task<IEnumerable<Tag>> GetWorkflowTagsAsync(string workflowId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update workflow tags
        /// </summary>
        /// <param name="workflowId">The workflow ID</param>
        /// <param name="request">Tag update request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated list of tags</returns>
        Task<IEnumerable<Tag>> UpdateWorkflowTagsAsync(string workflowId, UpdateWorkflowTagsRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Execution API

        /// <summary>
        /// Retrieve all executions
        /// </summary>
        /// <param name="options">Options for filtering executions</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of executions</returns>
        Task<ExecutionListResponse> GetExecutionsAsync(ListExecutionsOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a specific execution
        /// </summary>
        /// <param name="executionId">The execution ID</param>
        /// <param name="options">Options for getting the execution</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Execution details</returns>
        Task<Execution> GetExecutionAsync(int executionId, GetExecutionOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete an execution
        /// </summary>
        /// <param name="executionId">The execution ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deleted execution</returns>
        Task<Execution> DeleteExecutionAsync(int executionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retry an execution
        /// </summary>
        /// <param name="executionId">The execution ID</param>
        /// <param name="request">Retry options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>New execution from the retry</returns>
        Task<Execution> RetryExecutionAsync(int executionId, RetryExecutionRequest? request = null, CancellationToken cancellationToken = default);

        #endregion

        #region Credential API

        /// <summary>
        /// Create a new credential
        /// </summary>
        /// <param name="credential">Credential to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created credential response</returns>
        Task<CreateCredentialResponse> CreateCredentialAsync(Credential credential, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a credential
        /// </summary>
        /// <param name="credentialId">The credential ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deleted credential</returns>
        Task<Credential> DeleteCredentialAsync(string credentialId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get credential type schema
        /// </summary>
        /// <param name="credentialTypeName">The credential type name</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Credential type schema</returns>
        Task<CredentialTypeSchema> GetCredentialTypeSchemaAsync(string credentialTypeName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Transfer a credential to another project
        /// </summary>
        /// <param name="credentialId">The credential ID</param>
        /// <param name="request">Transfer request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task TransferCredentialAsync(string credentialId, TransferCredentialRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Tag API

        /// <summary>
        /// Retrieve all tags
        /// </summary>
        /// <param name="options">Pagination options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of tags</returns>
        Task<TagListResponse> GetTagsAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a specific tag
        /// </summary>
        /// <param name="tagId">The tag ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Tag details</returns>
        Task<Tag> GetTagAsync(string tagId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new tag
        /// </summary>
        /// <param name="tag">Tag to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created tag</returns>
        Task<Tag> CreateTagAsync(Tag tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing tag
        /// </summary>
        /// <param name="tagId">The tag ID</param>
        /// <param name="tag">Updated tag data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated tag</returns>
        Task<Tag> UpdateTagAsync(string tagId, Tag tag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a tag
        /// </summary>
        /// <param name="tagId">The tag ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Deleted tag</returns>
        Task<Tag> DeleteTagAsync(string tagId, CancellationToken cancellationToken = default);

        #endregion

        #region User API (Enterprise features)

        /// <summary>
        /// Retrieve all users
        /// </summary>
        /// <param name="options">Options for filtering users</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of users</returns>
        Task<UserListResponse> GetUsersAsync(ListUsersOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve a specific user
        /// </summary>
        /// <param name="userIdentifier">The user ID or email</param>
        /// <param name="options">Options for getting the user</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User details</returns>
        Task<User> GetUserAsync(string userIdentifier, GetUserOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create multiple users
        /// </summary>
        /// <param name="users">Users to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Creation results</returns>
        Task<IEnumerable<CreateUserResponse>> CreateUsersAsync(IEnumerable<CreateUserRequest> users, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="userIdentifier">The user ID or email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task DeleteUserAsync(string userIdentifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Change a user's global role
        /// </summary>
        /// <param name="userIdentifier">The user ID or email</param>
        /// <param name="request">Role change request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task ChangeUserRoleAsync(string userIdentifier, ChangeUserRoleRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Variable API

        /// <summary>
        /// Retrieve variables
        /// </summary>
        /// <param name="options">Options for filtering variables</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of variables</returns>
        Task<VariableListResponse> GetVariablesAsync(ListVariablesOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new variable
        /// </summary>
        /// <param name="variable">Variable to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task CreateVariableAsync(VariableCreateRequest variable, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing variable
        /// </summary>
        /// <param name="variableId">The variable ID</param>
        /// <param name="variable">Updated variable data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task UpdateVariableAsync(string variableId, VariableCreateRequest variable, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a variable
        /// </summary>
        /// <param name="variableId">The variable ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task DeleteVariableAsync(string variableId, CancellationToken cancellationToken = default);

        #endregion

        #region Project API

        /// <summary>
        /// Retrieve projects
        /// </summary>
        /// <param name="options">Pagination options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of projects</returns>
        Task<ProjectListResponse> GetProjectsAsync(PaginationOptions? options = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="project">Project to create</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task CreateProjectAsync(Project project, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update an existing project
        /// </summary>
        /// <param name="projectId">The project ID</param>
        /// <param name="project">Updated project data</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task UpdateProjectAsync(string projectId, Project project, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="projectId">The project ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Add users to a project
        /// </summary>
        /// <param name="projectId">The project ID</param>
        /// <param name="request">Users to add</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task AddUsersToProjectAsync(string projectId, AddUsersToProjectRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Remove a user from a project
        /// </summary>
        /// <param name="projectId">The project ID</param>
        /// <param name="userId">The user ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task RemoveUserFromProjectAsync(string projectId, string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Change a user's role in a project
        /// </summary>
        /// <param name="projectId">The project ID</param>
        /// <param name="userId">The user ID</param>
        /// <param name="request">Role change request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task ChangeUserRoleInProjectAsync(string projectId, string userId, ChangeProjectUserRoleRequest request, CancellationToken cancellationToken = default);

        #endregion

        #region Audit API

        /// <summary>
        /// Generate a security audit
        /// </summary>
        /// <param name="request">Audit request options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Audit report</returns>
        Task<AuditResponse> GenerateAuditAsync(GenerateAuditRequest? request = null, CancellationToken cancellationToken = default);

        #endregion

        #region Source Control API

        /// <summary>
        /// Pull changes from remote repository
        /// </summary>
        /// <param name="request">Pull request options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Import result</returns>
        Task<ImportResult> PullFromRepositoryAsync(PullRequest request, CancellationToken cancellationToken = default);

        #endregion
    }
}
