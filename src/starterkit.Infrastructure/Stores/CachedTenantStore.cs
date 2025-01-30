using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using starterkit.starterkit.Application.Persistence;
using starterkit.starterkit.Core.Modules.Common;


namespace starterkit.starterkit.Infrastructure.Stores
{
    public class CachedTenantStore : ITenantStore
    {
        private readonly IRootDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private const string TENANT_KEY_PREFIX = "tenant_";
        private const string ALL_TENANTS_KEY = "all_tenants";

        public CachedTenantStore(IRootDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<TenantInfo> GetTenantAsync(string identifier)
        {
            var cacheKey = $"{TENANT_KEY_PREFIX}{identifier.ToLower()}";

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