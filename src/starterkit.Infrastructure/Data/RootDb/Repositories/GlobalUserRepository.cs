using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;
using starterkit.Core.Interfaces.Repositories.Global;

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
    }
} 