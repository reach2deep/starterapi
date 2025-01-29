using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.MultiTenancy.Models;

namespace starterkit.Infrastructure.MultiTenancy.Stores
{
    public class CachedTenantStore : ITenantStore
    {
        private readonly RootDbContext _context;
        private readonly IMemoryCache _cache;
        private const string ALL_TENANTS_KEY = "all_tenants";
        private const string TENANT_KEY_PREFIX = "tenant_";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public CachedTenantStore(RootDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<TenantInfo> GetTenantAsync(string identifier)
        {
            var cacheKey = $"{TENANT_KEY_PREFIX}{identifier}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => 
                        t.Id.ToString().ToLower() == identifier.ToLower() ||
                        t.Name.ToLower() == identifier.ToLower() || 
                        t.DatabaseName.ToLower() == identifier.ToLower());

                if (tenant == null)
                    return null;

                return new TenantInfo
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    DatabaseName = tenant.DatabaseName,
                    ConnectionString = tenant.ConnectionString,
                    Status = tenant.Status
                };
            });
        }

        public async Task<IEnumerable<TenantInfo>> GetAllTenantsAsync()
        {
            return await _cache.GetOrCreateAsync(ALL_TENANTS_KEY, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = _cacheDuration;

                return await _context.Tenants
                    .Select(t => new TenantInfo
                    {
                        Id = t.Id,
                        Name = t.Name,
                        DatabaseName = t.DatabaseName,
                        ConnectionString = t.ConnectionString,
                        Status = t.Status
                    })
                    .ToListAsync();
            });
        }

        public async Task<bool> TenantExistsAsync(string identifier)
        {
            var tenant = await GetTenantAsync(identifier);
            return tenant != null;
        }
    }
} 