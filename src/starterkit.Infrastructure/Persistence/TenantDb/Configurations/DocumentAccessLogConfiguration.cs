using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Common.Documents.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the DocumentAccessLog entity in the tenant database
    /// </summary>
    public class DocumentAccessLogConfiguration : IEntityTypeConfiguration<DocumentAccessLog>
    {
        public void Configure(EntityTypeBuilder<DocumentAccessLog> builder)
        {
            builder.ToTable("DocumentAccessLogs");

            builder.HasKey(d => d.Id);
            
            builder.Property(d => d.DocumentId)
                .IsRequired();
                
            builder.Property(d => d.AccessedById)
                .IsRequired(false);
                
            builder.Property(d => d.IpAddress)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(d => d.AccessedAt)
                .IsRequired();
                
            builder.Property(d => d.AccessType)
                .IsRequired();
                
            // Relationships
            builder.HasOne(d => d.Document)
                .WithMany()
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);
                
            // Indexing for faster queries
            builder.HasIndex(d => d.DocumentId);
            builder.HasIndex(d => d.AccessedById);
            builder.HasIndex(d => d.AccessedAt);
        }
    }
} 