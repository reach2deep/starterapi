using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;

namespace starterkit.Infrastructure.Data.RootDb
{
    public class RootDbContext : DbContext
    {
        public RootDbContext(DbContextOptions<RootDbContext> options) : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<GlobalUser> GlobalUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RootDbContext).Assembly);
        }
    }
} 