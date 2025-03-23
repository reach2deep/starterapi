using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using starterkit.Core.Modules.Common.Documents.Entities;

namespace starterkit.Infrastructure.Persistence.TenantDb.Configurations
{
    /// <summary>
    /// Entity Framework configuration for the Document entity in the tenant database
    /// </summary>
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(d => d.Id);
            
            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(d => d.ContentType)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(d => d.Size)
                .IsRequired();
                
            builder.Property(d => d.BlobUrl)
                .IsRequired()
                .HasMaxLength(1000);
                
            builder.Property(d => d.BlobName)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(d => d.ContainerName)
                .IsRequired()
                .HasMaxLength(255);
                
            builder.Property(d => d.IsPublic)
                .IsRequired();
                
            builder.Property(d => d.ExpiryDate)
                .IsRequired(false);
                
            builder.Property(d => d.CreatedAt)
                .IsRequired();
                
            builder.Property(d => d.UpdatedAt)
                .IsRequired();
                
            builder.Property(d => d.TenantId)
                .IsRequired();
                
            builder.Property(d => d.ModuleType)
                .IsRequired();
                
            builder.Property(d => d.EntityId)
                .IsRequired();
                
            builder.Property(d => d.Status)
                .IsRequired();
                
            // Indexing for faster queries
            builder.HasIndex(d => d.TenantId);
            builder.HasIndex(d => d.EntityId);
            builder.HasIndex(d => d.Status);
            builder.HasIndex(d => new { d.EntityId, d.ModuleType });
        }
    }
} 