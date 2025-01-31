using Microsoft.EntityFrameworkCore;
using starterkit.Core.Modules.Global.Auth.Interfaces.Repositories;
using starterkit.Core.Modules.Global;
using starterkit.Infrastructure.Persistence.RootDb;

namespace starterkit.Infrastructure.Data.RootDb.Repositories
{
    public class GlobalUserRepository : IGlobalUserRepository
    {
        private readonly RootDbContext _context;

        public GlobalUserRepository(RootDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalUser?> GetByEmailAsync(string email)
        {
            return await _context.GlobalUsers
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<GlobalUser?> GetByIdAsync(Guid id)
        {
            return await _context.GlobalUsers
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<TenantUserMapping>> GetActiveTenantMappingsAsync(Guid userId)
        {
            return await _context.Set<TenantUserMapping>()
                .Include(t => t.Tenant)
                .Where(t => t.UserId == userId &&
                           t.IsActive &&
                           t.Tenant.Status == Core.Enums.TenantStatus.Active)
                .ToListAsync();
        }

        public async Task<TenantUserMapping?> GetTenantMappingAsync(Guid userId, Guid tenantId)
        {
            return await _context.Set<TenantUserMapping>()
                .Include(t => t.Tenant)
                .FirstOrDefaultAsync(t =>
                    t.TenantId == tenantId &&
                    t.UserId == userId &&
                    t.IsActive &&
                    t.Tenant.Status == Core.Enums.TenantStatus.Active);
        }

        public Task<IEnumerable<GlobalUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GlobalUser> CreateAsync(GlobalUser user)
        {
            throw new NotImplementedException();
        }

        public Task<GlobalUser> UpdateAsync(GlobalUser user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }
    }
}