using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using starterkit.Core.Modules.Tenant;

using starterkit.Application.Persistence;
using starterkit.Core.Modules.Tenant.PermissionManagement.Interfaces;

namespace starterkit.Infrastructure.Persistence.TenantDb.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<PermissionRepository> _logger;

        public PermissionRepository(ITenantDbContext context, ILogger<PermissionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _context.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Permission> GetByIdAsync(Guid id)
        {
            return await _context.Permissions
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<IEnumerable<Permission>> GetByModuleAsync(string module)
        {
            return await _context.Permissions
                .Where(p => p.Module == module && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Permission>> GetByRoleIdAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive)
                .Include(rp => rp.Permission)
                .Where(rp => rp.Permission.IsActive)
                .Select(rp => rp.Permission)
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Permissions
                .AnyAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<Permission> AddAsync(Permission permission)
        {
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<Permission> UpdateAsync(Permission permission)
        {
            _context.Set<Permission>().Update(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task DeleteAsync(Guid id)
        {
            var permission = await GetByIdAsync(id);
            if (permission != null)
            {
                permission.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
} 