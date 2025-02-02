using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;

namespace starterkit.Application.Modules.Tenant.RoleManagement.Interfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// Gets a role by ID
        /// </summary>
        Task<ApiResponse<RoleResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all roles
        /// </summary>
        Task<ApiResponse<IEnumerable<RoleResponse>>> GetAllAsync();

        /// <summary>
        /// Gets a paged list of roles
        /// </summary>
        Task<ApiResponse<PagedResponse<RoleResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new role
        /// </summary>
        Task<ApiResponse<RoleResponse>> CreateAsync(CreateRoleRequest request);

        /// <summary>
        /// Updates an existing role
        /// </summary>
        Task<ApiResponse<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request);

        /// <summary>
        /// Deletes a role
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Gets roles for a user
        /// </summary>
        Task<ApiResponse<IEnumerable<RoleResponse>>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Assigns permissions to a role
        /// </summary>
        Task<ApiResponse<bool>> AssignPermissionsAsync(Guid roleId, AssignPermissionsRequest request);

        /// <summary>
        /// Removes permissions from a role
        /// </summary>
        Task<ApiResponse<bool>> RemovePermissionsAsync(Guid roleId, RemovePermissionsRequest request);

        /// <summary>
        /// Gets all permissions for a role
        /// </summary>
        Task<ApiResponse<IEnumerable<PermissionResponse>>> GetPermissionsAsync(Guid roleId);

        /// <summary>
        /// Creates a copy of an existing role
        /// </summary>
        Task<ApiResponse<RoleResponse>> CopyRoleAsync(Guid sourceRoleId, CopyRoleRequest request);

        /// <summary>
        /// Assigns roles to a user
        /// </summary>
        Task<ApiResponse<bool>> AssignRolesToUserAsync(Guid userId, AssignUserRolesRequest request);

        /// <summary>
        /// Removes roles from a user
        /// </summary>
        Task<ApiResponse<bool>> RemoveRolesFromUserAsync(Guid userId, RemoveUserRolesRequest request);
    }
} 