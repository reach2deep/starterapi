using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class SocietyConfiguration : IEntityTypeConfiguration<Society>
    {
        public void Configure(EntityTypeBuilder<Society> builder)
        {
            builder.ToTable("Societies");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.RegistrationNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ContactEmail)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ContactPhone)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(x => x.Address)
                .WithMany()
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Blocks)
                .WithOne(x => x.Society)
                .HasForeignKey(x => x.SocietyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.FeatureAccess)
                .WithOne(x => x.Society)
                .HasForeignKey(x => x.SocietyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Subscriptions)
                .WithOne(x => x.Society)
                .HasForeignKey(x => x.SocietyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 