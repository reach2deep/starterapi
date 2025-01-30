using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Tenant;


namespace starterkit.Infrastructure.Data.TenantDb.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.ReplacedByToken)
                .HasMaxLength(100);

            builder.Property(r => r.RevokedReason)
                .HasMaxLength(200);

            // Configure relationship with User
            builder.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Create index on Token for faster lookups
            builder.HasIndex(r => r.Token)
                .IsUnique();
        }
    }
}