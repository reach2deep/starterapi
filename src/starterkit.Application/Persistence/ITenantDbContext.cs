using Microsoft.EntityFrameworkCore;
using starterkit.Core.Modules.Tenant;


namespace starterkit.Application.Persistence
{
    public interface ITenantDbContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}