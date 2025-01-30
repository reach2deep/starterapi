
using starterkit.starterkit.Core.Modules.Global;

namespace starterkit.starterkit.Application.Modules.Global.Auth.Interfaces
{
    public interface IGlobalUserRepository
    {
        Task<GlobalUser?> GetByEmailAsync(string email);
        Task<GlobalUser?> GetByIdAsync(Guid id);
        Task<List<TenantUserMapping>> GetActiveTenantMappingsAsync(Guid userId);
        Task<TenantUserMapping?> GetTenantMappingAsync(Guid userId, Guid tenantId);
    }
}