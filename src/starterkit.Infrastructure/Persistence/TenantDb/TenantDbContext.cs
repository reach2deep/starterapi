using Microsoft.EntityFrameworkCore;
using starterkit.Application.Persistence;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Society> Societies { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitOwnership> UnitOwnerships { get; set; }
        public DbSet<SocietySubscription> SocietySubscriptions { get; set; }
        public DbSet<FeatureAccess> FeatureAccesses { get; set; }
        public DbSet<UnitResident> UnitResidents { get; set; }

    
        


        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // You could add tenant validation here if needed
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}