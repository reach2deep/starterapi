using starterkit.Core.Modules.Global;

namespace starterkit.Core.Modules.Global.TenantManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing tenants in the root database
    /// </summary>
    public interface ITenantRepository
    {
        /// <summary>
        /// Creates a new tenant
        /// </summary>
        Task<Tenant> CreateAsync(Tenant tenant);

        /// <summary>
        /// Creates a tenant user mapping
        /// </summary>
        Task<TenantUserMapping> CreateTenantUserMappingAsync(TenantUserMapping mapping);

        /// <summary>
        /// Gets a tenant by ID
        /// </summary>
        Task<Tenant> GetByIdAsync(Guid id);

        /// <summary>
        /// Checks if a tenant exists by name
        /// </summary>
        Task<bool> ExistsByNameAsync(string name);

        /// <summary>
        /// Checks if a tenant exists by database name
        /// </summary>
        Task<bool> ExistsByDatabaseNameAsync(string dbName);

        /// <summary>
        /// Updates an existing tenant
        /// </summary>
        Task<Tenant> UpdateAsync(Tenant tenant);

        /// <summary>
        /// Deactivates a tenant
        /// </summary>
        Task<bool> DeactivateAsync(Guid id);

        /// <summary>
        /// Gets a paged list of tenants
        /// </summary>
        Task<(IEnumerable<Tenant> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Removes a tenant and related data (for cleanup in case of creation failure)
        /// </summary>
        Task RemoveAsync(Tenant tenant);
    }
} 