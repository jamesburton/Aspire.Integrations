using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace N8N.Client.Models
{
    /// <summary>
    /// User model
    /// </summary>
    public class User
    {
        /// <summary>
        /// User ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        /// <summary>
        /// User's first name (read-only)
        /// </summary>
        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        /// <summary>
        /// User's last name (read-only)
        /// </summary>
        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        /// <summary>
        /// Whether the user is pending invitation acceptance (read-only)
        /// </summary>
        [JsonPropertyName("isPending")]
        public bool? IsPending { get; set; }

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

        /// <summary>
        /// User's role (read-only)
        /// </summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    /// <summary>
    /// User list response
    /// </summary>
    public class UserListResponse : PaginatedResponse<User>
    {
        /// <summary>
        /// The users for this page
        /// </summary>
        [JsonPropertyName("data")]
        public new required IEnumerable<User> Data { get; set; }

        /// <summary>
        /// Cursor for the next page
        /// </summary>
        [JsonPropertyName("nextCursor")]
        public new string? NextCursor { get; set; }
    }

    /// <summary>
    /// Options for listing users
    /// </summary>
    public class ListUsersOptions : PaginationOptions
    {
        /// <summary>
        /// Whether to include the user's role
        /// </summary>
        public bool? IncludeRole { get; set; }

        /// <summary>
        /// Filter by project ID
        /// </summary>
        public string? ProjectId { get; set; }
    }

    /// <summary>
    /// Options for getting a single user
    /// </summary>
    public class GetUserOptions
    {
        /// <summary>
        /// Whether to include the user's role
        /// </summary>
        public bool? IncludeRole { get; set; }
    }

    /// <summary>
    /// Request for creating users
    /// </summary>
    public class CreateUserRequest
    {
        /// <summary>
        /// User email
        /// </summary>
        [JsonPropertyName("email")]
        public required string Email { get; set; }

        /// <summary>
        /// User role
        /// </summary>
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    /// <summary>
    /// Response when creating a user
    /// </summary>
    public class CreateUserResponse
    {
        /// <summary>
        /// User information
        /// </summary>
        [JsonPropertyName("user")]
        public UserCreationInfo? User { get; set; }

        /// <summary>
        /// Error message if creation failed
        /// </summary>
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }

    /// <summary>
    /// User creation information
    /// </summary>
    public class UserCreationInfo
    {
        /// <summary>
        /// User ID
        /// </summary>
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        /// <summary>
        /// User email
        /// </summary>
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        /// <summary>
        /// Invitation accept URL
        /// </summary>
        [JsonPropertyName("inviteAcceptUrl")]
        public string? InviteAcceptUrl { get; set; }

        /// <summary>
        /// Whether invitation email was sent
        /// </summary>
        [JsonPropertyName("emailSent")]
        public bool EmailSent { get; set; }
    }

    /// <summary>
    /// Request for changing a user's role
    /// </summary>
    public class ChangeUserRoleRequest
    {
        /// <summary>
        /// New role name
        /// </summary>
        [JsonPropertyName("newRoleName")]
        public required string NewRoleName { get; set; }
    }

    /// <summary>
    /// Role model
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Role ID (read-only)
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Role name (read-only)
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Role scope (read-only)
        /// </summary>
        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

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
}
