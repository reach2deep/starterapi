using starterkit.Core.Entities.Tenant;

namespace starterkit.Core.Interfaces.Repositories.Tenant
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> GetByIdAsync(Guid id);
        Task<UserProfile> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile> UpdateAsync(UserProfile profile);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsByUserIdAsync(Guid userId);
    }
} 