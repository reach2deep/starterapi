using starterkit.Infrastructure.MultiTenancy.Models;

namespace starterkit.Infrastructure.MultiTenancy.Stores
{
    public interface ITenantStore
    {
        Task<TenantInfo> GetTenantAsync(string identifier);
        Task<IEnumerable<TenantInfo>> GetAllTenantsAsync();
        Task<bool> TenantExistsAsync(string identifier);
    }
} 