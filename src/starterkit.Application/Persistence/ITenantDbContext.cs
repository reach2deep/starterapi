using Microsoft.EntityFrameworkCore;
using starterkit.Core.Modules.Common.Documents.Entities;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;


namespace starterkit.Application.Persistence
{
    public interface ITenantDbContext : IDisposable
    {
        DbSet<User> Users { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Address> Addresses { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<Permission> Permissions { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        DbSet<RolePermission> RolePermissions { get; set; }
        DbSet<Society> Societies { get; set; }
        DbSet<Block> Blocks { get; set; }
        DbSet<Floor> Floors { get; set; }
        DbSet<Unit> Units { get; set; }
        DbSet<UnitOwnership> UnitOwnerships { get; set; }
        DbSet<UnitResident> UnitResidents { get; set; }
        DbSet<SocietySubscription> SocietySubscriptions { get; set; }
        DbSet<FeatureAccess> FeatureAccesses { get; set; }
        DbSet<LeaseAgreement> LeaseAgreements { get; set; }
        DbSet<RentPayment> RentPayments { get; set; }
        // Document management entities
        DbSet<Document> Documents { get; set; }
        DbSet<DocumentAccessLog> DocumentAccessLogs { get; set; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}