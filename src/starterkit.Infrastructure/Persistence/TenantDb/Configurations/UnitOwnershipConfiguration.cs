using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class UnitOwnershipConfiguration : IEntityTypeConfiguration<UnitOwnership>
    {
        public void Configure(EntityTypeBuilder<UnitOwnership> builder)
        {
            builder.ToTable("UnitOwnerships");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(x => x.Unit)
                .WithMany(x => x.Ownerships)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create an index on UnitId and StartDate for efficient querying
            builder.HasIndex(x => new { x.UnitId, x.StartDate });

            // Create an index on OwnerId for efficient querying
            builder.HasIndex(x => x.OwnerId);
        }
    }
} 