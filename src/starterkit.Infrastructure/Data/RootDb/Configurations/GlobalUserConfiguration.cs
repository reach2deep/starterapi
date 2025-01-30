using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Entities.Global;

namespace starterkit.starterkit.Infrastructure.Data.RootDb.Configurations
{
    public class GlobalUserConfiguration : IEntityTypeConfiguration<GlobalUser>
    {
        public void Configure(EntityTypeBuilder<GlobalUser> builder)
        {
            builder.ToTable("GlobalUsers");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.MobileNumber)
                .IsRequired(false)
                .HasMaxLength(20);

            builder.Property(u => u.PasswordHash)
                .IsRequired();

            builder.Property(u => u.UserType)
                .IsRequired();

            builder.Property(u => u.Status)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}