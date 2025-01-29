using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Tenant;

namespace starterkit.Core.Interfaces.Data
{
    public interface ITenantDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Address> Addresses { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 