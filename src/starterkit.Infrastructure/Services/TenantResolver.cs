using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using starterkit.Infrastructure.MultiTenancy.Models;
using starterkit.Infrastructure.MultiTenancy.Stores;
using starterkit.starterkit.Core.Modules.Common;
using starterkit.starterkit.Infrastructure.Stores;
using System.Security.Claims;

namespace starterkit.starterkit.Infrastructure.Services
{
    public interface ITenantResolver
    {
        Task<TenantInfo> ResolveTenantAsync(HttpContext context);
    }

    public class TenantResolver : ITenantResolver
    {
        private readonly ITenantStore _tenantStore;
        private readonly ILogger<TenantResolver> _logger;

        public TenantResolver(
            ITenantStore tenantStore,
            ILogger<TenantResolver> logger)
        {
            _tenantStore = tenantStore;
            _logger = logger;
        }

        public async Task<TenantInfo> ResolveTenantAsync(HttpContext context)
        {
            string tenantId = null;

            // First try from header
            tenantId = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
            _logger.LogInformation("Tenant ID from header: {TenantId}", tenantId);

            if (string.IsNullOrEmpty(tenantId))
            {
                // Log all claims for debugging
                if (context.User?.Identity?.IsAuthenticated == true)
                {
                    // _logger.LogInformation("User is authenticated. Available claims:");
                    // foreach (var claim in context.User.Claims)
                    // {
                    //     _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
                    // }

                    // Try from JWT token claims
                    tenantId = context.User?.FindFirst("tenant_id")?.Value
                           ?? context.User?.FindFirst("tenant-id")?.Value
                           ?? context.User?.FindFirst("TenantId")?.Value;
                    _logger.LogInformation("Tenant ID from JWT claim: {TenantId}", tenantId);
                }
                else
                {
                    _logger.LogWarning("User is not authenticated");
                }
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                // Try from subdomain
                var host = context.Request.Host.Value;
                var subdomain = host.Split('.').FirstOrDefault();

                if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
                {
                    tenantId = subdomain;
                    _logger.LogInformation("Tenant ID from subdomain: {TenantId}", tenantId);
                }
            }

            // Skip tenant resolution for global endpoints
            if (context.Request.Path.StartsWithSegments("/api/v1/global"))
            {
                _logger.LogInformation("Skipping tenant resolution for global endpoint");
                return null;
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                _logger.LogWarning("No tenant ID found from any source");
                return null;
            }

            var tenant = await _tenantStore.GetTenantAsync(tenantId);
            _logger.LogInformation("Resolved tenant: {TenantInfo}", tenant != null ? "Found" : "Not Found");
            return tenant;
        }
    }
}