using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Global;

namespace starterkit.Infrastructure.Persistence.RootDb.Configurations
{
    public class LoginActivityConfiguration : IEntityTypeConfiguration<LoginActivity>
    {
        public void Configure(EntityTypeBuilder<LoginActivity> builder)
        {
            builder.ToTable("LoginActivities");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IpAddress).HasMaxLength(50);
            builder.Property(x => x.UserAgent).HasMaxLength(500);
            
            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 