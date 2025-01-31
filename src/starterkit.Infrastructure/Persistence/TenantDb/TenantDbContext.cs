using Microsoft.EntityFrameworkCore;
using starterkit.Application.Persistence;
using starterkit.Core.Modules.Tenant;
using starterkit.Infrastructure.Data.TenantDb.Configurations;
using starterkit.Infrastructure.Persistence.TenantDb.Configurations;

namespace starterkit.Infrastructure.Persistence.TenantDb
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
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply only tenant database configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // You could add tenant validation here if needed
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}