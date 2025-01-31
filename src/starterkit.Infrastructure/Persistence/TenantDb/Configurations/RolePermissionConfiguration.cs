using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");
            builder.HasKey(x => x.Id);

            // Create unique constraint on RoleId and PermissionId
            builder.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();

            // Configure relationships
            builder.HasOne(x => x.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Permission)
                .WithMany()
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 