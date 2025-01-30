using Microsoft.EntityFrameworkCore;

using starterkit.starterkit.Core.Modules.Global;

namespace starterkit.starterkit.Application.Persistence
{
    public interface IRootDbContext : IDisposable
    {
        DbSet<GlobalUser> GlobalUsers { get; set; }
        DbSet<TenantUserMapping> TenantUserMappings { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}