using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class FloorConfiguration : IEntityTypeConfiguration<Floor>
    {
        public void Configure(EntityTypeBuilder<Floor> builder)
        {
            builder.ToTable("Floors");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(x => x.Block)
                .WithMany(x => x.Floors)
                .HasForeignKey(x => x.BlockId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Units)
                .WithOne(x => x.Floor)
                .HasForeignKey(x => x.FloorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create a unique index on BlockId and FloorNumber
            builder.HasIndex(x => new { x.BlockId, x.FloorNumber })
                .IsUnique();
        }
    }
} 