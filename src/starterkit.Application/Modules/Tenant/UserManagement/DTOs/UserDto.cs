using starterkit.Core.Enums;


namespace starterkit.Application.Modules.Tenant.UserManagement.DTOs
{
    /// <summary>
    /// Response DTO for role information in user response
    /// </summary>
    public class UserRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    /// <summary>
    /// Response DTO for user information
    /// </summary>
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public UserStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserProfileResponseDto Profile { get; set; }
        public List<UserRoleDto> Roles { get; set; } = new();
    }

    /// <summary>
    /// Request DTO for creating a new user
    /// </summary>
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
        public AddressDto Address { get; set; }
    }

    /// <summary>
    /// Request DTO for updating an existing user
    /// </summary>
    public class UpdateUserRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserStatus Status { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
        public AddressDto Address { get; set; }
    }

    /// <summary>
    /// Request DTO for changing user password
    /// </summary>
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// Request DTO for admin resetting user password
    /// </summary>
    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; }
    }


} 