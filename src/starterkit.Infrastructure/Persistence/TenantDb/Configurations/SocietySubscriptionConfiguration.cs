using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    public class SocietySubscriptionConfiguration : IEntityTypeConfiguration<SocietySubscription>
    {
        public void Configure(EntityTypeBuilder<SocietySubscription> builder)
        {
            builder.ToTable("SocietySubscriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PlanType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne(x => x.Society)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.SocietyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 