using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class LeaseAgreementConfiguration : IEntityTypeConfiguration<LeaseAgreement>
    {
        public void Configure(EntityTypeBuilder<LeaseAgreement> builder)
        {
            builder.ToTable("LeaseAgreements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PaymentFrequency)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.RentAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.SecurityDeposit)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Configure relationships
            builder.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Tenant)
                .WithMany()
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.RentPayments)
                .WithOne(x => x.LeaseAgreement)
                .HasForeignKey(x => x.LeaseAgreementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create indexes for efficient querying
            builder.HasIndex(x => x.UnitId);
            builder.HasIndex(x => x.OwnerId);
            builder.HasIndex(x => x.TenantId);
            builder.HasIndex(x => x.Status);
        }
    }
} 