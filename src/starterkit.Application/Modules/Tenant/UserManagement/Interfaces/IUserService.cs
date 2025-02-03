using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.UserManagement.Interfaces
{
    /// <summary>
    /// Service interface for managing users in tenant context
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets a user by ID
        /// </summary>
        Task<ApiResponse<UserResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all users
        /// </summary>
        Task<ApiResponse<IEnumerable<UserResponse>>> GetAllAsync();

        /// <summary>
        /// Gets a paged list of users
        /// </summary>
        Task<ApiResponse<PagedResponse<UserResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new user
        /// </summary>
        Task<ApiResponse<UserResponse>> CreateAsync(CreateUserRequest request);

        /// <summary>
        /// Updates an existing user
        /// </summary>
        Task<ApiResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request);

        /// <summary>
        /// Deletes a user
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Changes user password
        /// </summary>
        Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);

        /// <summary>
        /// Admin resets user password
        /// </summary>
        Task<ApiResponse<bool>> ResetPasswordAsync(Guid userId, ResetPasswordRequest request);
    }
} 