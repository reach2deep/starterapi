using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;

namespace starterkit.Application.Modules.Tenant.UserManagement.DTOs
{
    /// <summary>
    /// Response DTO containing user's roles and unique permissions for RBAC
    /// </summary>
    public class UserPermissionsResponse
    {
        /// <summary>
        /// List of all roles assigned to the user
        /// </summary>
        public List<RoleResponse> Roles { get; set; } = new();

        /// <summary>
        /// List of unique permissions combined from all user roles.
        /// If a permission exists in multiple roles, it will appear only once.
        /// Permissions are compared based on their Module and Action combination.
        /// </summary>
        public List<PermissionResponse> UniquePermissions { get; set; } = new();
    }
} 