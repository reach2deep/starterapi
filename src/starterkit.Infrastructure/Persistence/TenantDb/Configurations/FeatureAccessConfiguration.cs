using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class FeatureAccessConfiguration : IEntityTypeConfiguration<FeatureAccess>
    {
        public void Configure(EntityTypeBuilder<FeatureAccess> builder)
        {
            builder.ToTable("FeatureAccesses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.FeatureKey)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(x => x.Society)
                .WithMany(x => x.FeatureAccess)
                .HasForeignKey(x => x.SocietyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create a unique index on SocietyId and FeatureKey
            builder.HasIndex(x => new { x.SocietyId, x.FeatureKey })
                .IsUnique();
        }
    }
} 