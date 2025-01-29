using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;

namespace starterkit.Core.Interfaces.Data
{
    public interface IRootDbContext
    {
        DbSet<GlobalUser> GlobalUsers { get; set; }
        DbSet<TenantUserMapping> TenantUserMappings { get; set; }
        DbSet<Tenant> Tenants { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 