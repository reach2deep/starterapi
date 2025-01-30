using starterkit.Core.Entities.Global;

namespace starterkit.Core.Interfaces.Repositories.Global
{
    public interface IGlobalUserRepository
    {
        Task<GlobalUser?> GetByEmailAsync(string email);
        Task<GlobalUser?> GetByIdAsync(Guid id);
        Task<List<TenantUserMapping>> GetActiveTenantMappingsAsync(Guid userId);
        Task<TenantUserMapping?> GetTenantMappingAsync(Guid userId, Guid tenantId);
    }
} 