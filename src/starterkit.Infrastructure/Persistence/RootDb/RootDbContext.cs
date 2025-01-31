using Microsoft.EntityFrameworkCore;
using starterkit.Application.Persistence;
using starterkit.Core.Modules.Global;
using starterkit.Infrastructure.Data.RootDb.Configurations;
using starterkit.Infrastructure.Persistence.RootDb.Configurations;

namespace starterkit.Infrastructure.Persistence.RootDb
{
    public class RootDbContext : DbContext, IRootDbContext
    {
        public RootDbContext(DbContextOptions<RootDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<GlobalUser> GlobalUsers { get; set; }
        public DbSet<TenantUserMapping> TenantUserMappings { get; set; }
        public DbSet<LoginActivity> LoginActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply only root database configurations
            modelBuilder.ApplyConfiguration(new GlobalUserConfiguration());
            modelBuilder.ApplyConfiguration(new TenantConfiguration());
            modelBuilder.ApplyConfiguration(new TenantUserMappingConfiguration());
            modelBuilder.ApplyConfiguration(new LoginActivityConfiguration());
        }
    }
}