using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class UnitResidentConfiguration : IEntityTypeConfiguration<UnitResident>
    {
        public void Configure(EntityTypeBuilder<UnitResident> builder)
        {
            builder.ToTable("UnitResidents");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RelationType)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(x => x.Unit)
                .WithMany(x => x.Residents)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Resident)
                .WithMany()
                .HasForeignKey(x => x.ResidentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create an index on UnitId and StartDate for efficient querying
            builder.HasIndex(x => new { x.UnitId, x.StartDate });

            // Create an index on ResidentId for efficient querying
            builder.HasIndex(x => x.ResidentId);

            // Create an index on IsPrimary and UnitId for efficient querying of primary residents
            builder.HasIndex(x => new { x.IsPrimary, x.UnitId });
        }
    }
} 