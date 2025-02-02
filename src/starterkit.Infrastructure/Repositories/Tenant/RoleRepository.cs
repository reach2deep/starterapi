using Microsoft.EntityFrameworkCore;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;
using starterkit.Application.Persistence;

namespace starterkit.Infrastructure.Repositories.Tenant
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ITenantDbContext _context;

        public RoleRepository(ITenantDbContext context)
        {
            _context = context;
        }

        public async Task<Role> GetByIdAsync(Guid id)
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Role> CreateAsync(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            role.UpdatedAt = DateTime.UtcNow;
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await GetByIdAsync(id);
            if (role != null)
            {
                role.IsActive = false;
                role.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.Roles
                .AnyAsync(r => r.Name.ToLower() == name.ToLower() && r.IsActive);
        }

        public async Task<IEnumerable<Role>> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == userId && ur.Role.IsActive)
                .Select(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Role> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(r => r.IsActive);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(r => r.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task AssignPermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds)
        {
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            _context.RolePermissions.RemoveRange(existingPermissions);

            var newPermissions = permissionIds.Select(pid => new RolePermission
            {
                RoleId = roleId,
                PermissionId = pid
            });

            await _context.RolePermissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePermissionsAsync(Guid roleId, IEnumerable<Guid> permissionIds)
        {
            var permissionsToRemove = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                .ToListAsync();

            _context.RolePermissions.RemoveRange(permissionsToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Permission>> GetPermissionsAsync(Guid roleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId && rp.Permission.IsActive)
                .Select(rp => rp.Permission)
                .ToListAsync();
        }
    }
} 