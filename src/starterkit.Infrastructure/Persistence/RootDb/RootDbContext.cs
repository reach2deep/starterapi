using Microsoft.EntityFrameworkCore;
using starterkit.starterkit.Application.Persistence;
using starterkit.starterkit.Core.Modules.Global;
using starterkit.starterkit.Infrastructure.Data.RootDb.Configurations;


namespace starterkit.starterkit.Infrastructure.Data.RootDb
{
    public class RootDbContext : DbContext, IRootDbContext
    {
        public RootDbContext(DbContextOptions<RootDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<GlobalUser> GlobalUsers { get; set; }
        public DbSet<TenantUserMapping> TenantUserMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply only root database configurations
            modelBuilder.ApplyConfiguration(new GlobalUserConfiguration());
            modelBuilder.ApplyConfiguration(new TenantConfiguration());
            modelBuilder.ApplyConfiguration(new TenantUserMappingConfiguration());
        }
    }
}