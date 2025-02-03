using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class BlockConfiguration : IEntityTypeConfiguration<Block>
    {
        public void Configure(EntityTypeBuilder<Block> builder)
        {
            builder.ToTable("Blocks");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasOne(x => x.Society)
                .WithMany(x => x.Blocks)
                .HasForeignKey(x => x.SocietyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Floors)
                .WithOne(x => x.Block)
                .HasForeignKey(x => x.BlockId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 