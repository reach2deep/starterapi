using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Tenant;
using starterkit.Core.Interfaces.Data;
using starterkit.Infrastructure.Data.TenantDb.Configurations;

namespace starterkit.Infrastructure.Data.TenantDb
{
    public class TenantDbContext : DbContext, ITenantDbContext
    {
        private readonly string _tenantId;

        public TenantDbContext(DbContextOptions<TenantDbContext> options, string tenantId) : base(options)
        {
            _tenantId = tenantId;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Address> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply only tenant database configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // You could add tenant validation here if needed
            return base.SaveChangesAsync(cancellationToken);
        }
    }
} 