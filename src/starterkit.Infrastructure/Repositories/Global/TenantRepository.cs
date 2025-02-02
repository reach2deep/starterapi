using Microsoft.EntityFrameworkCore;
using starterkit.Application.Persistence;
using starterkit.Core.Modules.Global;
using starterkit.Core.Modules.Global.TenantManagement.Interfaces.Repositories;

namespace starterkit.Infrastructure.Repositories.Global
{
    /// <summary>
    /// Implementation of ITenantRepository for managing tenants in the root database
    /// </summary>
    public class TenantRepository : ITenantRepository
    {
        private readonly IRootDbContext _context;

        public TenantRepository(IRootDbContext context)
        {
            _context = context;
        }

        public async Task<Core.Modules.Global.Tenant> CreateAsync(Core.Modules.Global.Tenant tenant)
        {
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<TenantUserMapping> CreateTenantUserMappingAsync(TenantUserMapping mapping)
        {
            _context.TenantUserMappings.Add(mapping);
            await _context.SaveChangesAsync();
            return mapping;
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null) return false;

            tenant.IsActive = false;
            tenant.Status = Core.Enums.TenantStatus.Inactive;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByDatabaseNameAsync(string dbName)
        {
            return await _context.Tenants.AnyAsync(t => t.DatabaseName == dbName);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Tenants.AnyAsync(t => t.Name == name);
        }

        public async Task<Core.Modules.Global.Tenant> GetByIdAsync(Guid id)
        {
            return await _context.Tenants.FindAsync(id);
        }

        public async Task<(IEnumerable<Core.Modules.Global.Tenant> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Tenants.AsNoTracking();
            var totalCount = await query.CountAsync();
            
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task RemoveAsync(Core.Modules.Global.Tenant tenant)
        {
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();
        }

        public async Task<Core.Modules.Global.Tenant> UpdateAsync(Core.Modules.Global.Tenant tenant)
        {
            _context.Set<Core.Modules.Global.Tenant>().Update(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }
    }
} 