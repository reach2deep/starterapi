using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Entities.Tenant;

namespace starterkit.Infrastructure.Data.TenantDb.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("UserProfiles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Address)
                .HasMaxLength(500);

            builder.Property(p => p.City)
                .HasMaxLength(100);

            builder.Property(p => p.Country)
                .HasMaxLength(100);

            builder.Property(p => p.PostalCode)
                .HasMaxLength(20);

            builder.Property(p => p.ProfilePictureUrl)
                .HasMaxLength(1000);

            // UserId is required and is set up as foreign key in UserConfiguration
            builder.Property(p => p.UserId)
                .IsRequired();
        }
    }
} 