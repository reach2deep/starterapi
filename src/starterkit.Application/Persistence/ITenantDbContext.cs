using Microsoft.EntityFrameworkCore;
using starterkit.starterkit.Core.Modules.Tenant;


namespace starterkit.starterkit.Application.Persistence
{
    public interface ITenantDbContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}