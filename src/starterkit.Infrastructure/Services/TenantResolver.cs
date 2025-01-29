using Microsoft.AspNetCore.Http;
using starterkit.Infrastructure.MultiTenancy.Models;
using starterkit.Infrastructure.MultiTenancy.Stores;

namespace starterkit.Infrastructure.Services
{
    public interface ITenantResolver
    {
        Task<TenantInfo> ResolveTenantAsync(HttpContext context);
    }

    public class TenantResolver : ITenantResolver
    {
        private readonly ITenantStore _tenantStore;

        public TenantResolver(ITenantStore tenantStore)
        {
            _tenantStore = tenantStore;
        }

        public async Task<TenantInfo> ResolveTenantAsync(HttpContext context)
        {
            // First try from header
            var tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(tenantId))
            {
                // Try from subdomain
                var host = context.Request.Host.Value;
                var subdomain = host.Split('.').FirstOrDefault();
                
                if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
                {
                    tenantId = subdomain;
                }
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                return null;
            }

            return await _tenantStore.GetTenantAsync(tenantId);
        }
    }
} 