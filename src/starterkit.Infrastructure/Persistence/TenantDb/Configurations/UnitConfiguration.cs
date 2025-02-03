using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class UnitConfiguration : IEntityTypeConfiguration<Unit>
    {
        public void Configure(EntityTypeBuilder<Unit> builder)
        {
            builder.ToTable("Units");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UnitNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(x => x.Floor)
                .WithMany(x => x.Units)
                .HasForeignKey(x => x.FloorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Ownerships)
                .WithOne(x => x.Unit)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Residents)
                .WithOne(x => x.Unit)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create a unique index on FloorId and UnitNumber
            builder.HasIndex(x => new { x.FloorId, x.UnitNumber })
                .IsUnique();
        }
    }
} 