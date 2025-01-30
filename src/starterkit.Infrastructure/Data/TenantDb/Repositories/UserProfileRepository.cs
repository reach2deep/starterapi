using Microsoft.EntityFrameworkCore;
using starterkit.starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.starterkit.Application.Persistence;
using starterkit.starterkit.Core.Modules.Tenant;

namespace starterkit.starterkit.Infrastructure.Data.TenantDb.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ITenantDbContext _context;

        public UserProfileRepository(ITenantDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile> GetByIdAsync(Guid id)
        {
            return await _context.UserProfiles
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<UserProfile> GetByUserIdAsync(Guid userId)
        {
            return await _context.UserProfiles
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<UserProfile>> GetAllAsync()
        {
            return await _context.UserProfiles
                .Include(p => p.Address)
                .ToListAsync();
        }

        public async Task<UserProfile> UpdateAsync(UserProfile profile)
        {
            _context.Set<UserProfile>().Update(profile);
            if (profile.Address != null)
            {
                if (profile.Address.Id == Guid.Empty)
                {
                    _context.Set<Address>().Add(profile.Address);
                }
                else
                {
                    _context.Set<Address>().Update(profile.Address);
                }
            }
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.UserProfiles.AnyAsync(p => p.Id == id);
        }

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
        {
            return await _context.UserProfiles.AnyAsync(p => p.UserId == userId);
        }
    }
}