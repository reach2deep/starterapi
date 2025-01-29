using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Tenant;

namespace starterkit.Infrastructure.Data.TenantDb
{
    public class TenantDbContext : DbContext
    {
        private readonly string _tenantId;

        public TenantDbContext(DbContextOptions<TenantDbContext> options, string tenantId) : base(options)
        {
            _tenantId = tenantId;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // You could add tenant validation here if needed
            return base.SaveChangesAsync(cancellationToken);
        }
    }
} 