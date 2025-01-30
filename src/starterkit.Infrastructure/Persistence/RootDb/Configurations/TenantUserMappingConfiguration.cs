using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Global;


namespace starterkit.Infrastructure.Data.RootDb.Configurations
{
    public class TenantUserMappingConfiguration : IEntityTypeConfiguration<TenantUserMapping>
    {
        public void Configure(EntityTypeBuilder<TenantUserMapping> builder)
        {
            builder.ToTable("TenantUserMappings");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Role)
                .IsRequired()
                .HasMaxLength(50);

            // Configure relationships
            builder.HasOne(t => t.Tenant)
                .WithMany()
                .HasForeignKey(t => t.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create unique index for tenant-user combination
            builder.HasIndex(t => new { t.TenantId, t.UserId })
                .IsUnique();
        }
    }
}