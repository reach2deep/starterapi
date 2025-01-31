using starterkit.Core.Modules.Tenant;

namespace starterkit.Core.Modules.Tenant.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<IEnumerable<Permission>> GetAllAsync();
        Task<Permission> GetByIdAsync(Guid id);
        Task<IEnumerable<Permission>> GetByModuleAsync(string module);
        Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId);
        Task<bool> ExistsAsync(Guid id);
        Task<Permission> AddAsync(Permission permission);
        Task<Permission> UpdateAsync(Permission permission);
        Task DeleteAsync(Guid id);
    }
} 