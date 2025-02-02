using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant;


namespace starterkit.Infrastructure.Data.TenantDb.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ProfilePictureUrl)
                .HasMaxLength(1000);

            // UserId is required and is set up as foreign key in UserConfiguration
            builder.Property(p => p.UserId)
                .IsRequired();

            // AddressId is optional
            builder.Property(p => p.AddressId)
                .IsRequired(false);

            // Configure one-to-one relationship with Address
            builder.HasOne(p => p.Address)
                .WithOne()
                .HasForeignKey<UserProfile>(p => p.AddressId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}