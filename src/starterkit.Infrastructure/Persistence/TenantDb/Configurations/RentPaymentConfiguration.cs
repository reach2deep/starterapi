using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class RentPaymentConfiguration : IEntityTypeConfiguration<RentPayment>
    {
        public void Configure(EntityTypeBuilder<RentPayment> builder)
        {
            builder.ToTable("RentPayments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(x => x.PaymentMode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.TransactionReference)
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);

            // Configure relationships
            builder.HasOne(x => x.LeaseAgreement)
                .WithMany(x => x.RentPayments)
                .HasForeignKey(x => x.LeaseAgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create indexes for efficient querying
            builder.HasIndex(x => x.LeaseAgreementId);
            builder.HasIndex(x => x.DueDate);
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.TransactionReference)
                .IsUnique()
                .HasFilter("[TransactionReference] IS NOT NULL");
        }
    }
} 