using starterkit.Core.Modules.Tenant;

namespace starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing roles
    /// </summary>
    public interface IRoleRepository
    {
        /// <summary>
        /// Gets a role by ID
        /// </summary>
        Task<Role> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all roles
        /// </summary>
        Task<IEnumerable<Role>> GetAllAsync();

        /// <summary>
        /// Creates a new role
        /// </summary>
        Task<Role> CreateAsync(Role role);

        /// <summary>
        /// Updates an existing role
        /// </summary>
        Task<Role> UpdateAsync(Role role);

        /// <summary>
        /// Deletes a role
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a role exists by name
        /// </summary>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// Gets roles by user ID
        /// </summary>
        Task<IEnumerable<Role>> GetByUserIdAsync(Guid userId);

        /// <summary>
        /// Gets a paged list of roles
        /// </summary>
        Task<(IEnumerable<Role> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Assigns permissions to a role
        /// </summary>
        Task AssignPermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds);

        /// <summary>
        /// Removes permissions from a role
        /// </summary>
        Task RemovePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds);

        /// <summary>
        /// Gets all permissions for a role
        /// </summary>
        Task<IEnumerable<Permission>> GetPermissionsAsync(Guid roleId);

        /// <summary>
        /// Creates a new user role mapping
        /// </summary>
        Task<UserRole> CreateUserRoleAsync(UserRole userRole);

        /// <summary>
        /// Removes user role mappings
        /// </summary>
        Task RemoveUserRolesAsync(Guid userId, IEnumerable<Guid> roleIds);
    }
} 