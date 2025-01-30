
using starterkit.starterkit.Core.Modules.Common;

namespace starterkit.starterkit.Infrastructure.Stores
{
    public interface ITenantStore
    {
        Task<TenantInfo> GetTenantAsync(string identifier);
        Task<bool> TenantExistsAsync(string identifier);
        Task<IEnumerable<TenantInfo>> GetAllTenantsAsync();
    }
}