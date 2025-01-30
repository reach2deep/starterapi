using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Global;


namespace starterkit.Infrastructure.Data.RootDb.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.DatabaseName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.ConnectionString)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.Status)
                .IsRequired();

            builder.HasIndex(t => t.Name)
                .IsUnique();

            builder.HasIndex(t => t.DatabaseName)
                .IsUnique();
        }
    }
}