using starterkit.Infrastructure.MultiTenancy.Models;

namespace starterkit.Infrastructure.MultiTenancy.Stores
{
    public interface ITenantStore
    {
        Task<TenantInfo> GetTenantAsync(string identifier);
        Task<bool> TenantExistsAsync(string identifier);
        Task<IEnumerable<TenantInfo>> GetAllTenantsAsync();
    }
} 