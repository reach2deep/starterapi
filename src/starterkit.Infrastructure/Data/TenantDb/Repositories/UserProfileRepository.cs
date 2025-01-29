using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Tenant;
using starterkit.Core.Interfaces.Repositories.Tenant;

namespace starterkit.Infrastructure.Data.TenantDb.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly TenantDbContext _context;

        public UserProfileRepository(TenantDbContext context)
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
            _context.Entry(profile).State = EntityState.Modified;
            if (profile.Address != null)
            {
                _context.Entry(profile.Address).State = profile.Address.Id == Guid.Empty ? 
                    EntityState.Added : EntityState.Modified;
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