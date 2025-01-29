using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;

namespace starterkit.Core.Interfaces.Data
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