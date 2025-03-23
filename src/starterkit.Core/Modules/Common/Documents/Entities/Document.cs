using System;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Common.Documents.Enums;

namespace starterkit.Core.Modules.Common.Documents.Entities
{
    /// <summary>
    /// Represents a document stored in Azure Blob Storage
    /// </summary>
    public class Document : BaseEntity
    {
        /// <summary>
        /// Original filename of the document
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// MIME type of the document
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Size of the document in bytes
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// URL to access the blob in Azure Storage
        /// </summary>
        public string BlobUrl { get; set; }

        /// <summary>
        /// Unique name of the blob in the container
        /// </summary>
        public string BlobName { get; set; }

        /// <summary>
        /// Name of the Azure Blob container
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Indicates if the document is publicly accessible
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Expiration date for temporary URLs (nullable)
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Date and time when the document was created
        /// </summary>
        public new DateTime CreatedAt { get; set; }

        /// <summary>
        /// Date and time when the document was last updated
        /// </summary>
        public new DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Associated tenant ID (null for global files)
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// Type of module the document belongs to
        /// </summary>
        public ModuleType ModuleType { get; set; }

        /// <summary>
        /// ID of the entity this document is associated with
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Current status of the document
        /// </summary>
        public DocumentStatus Status { get; set; } = DocumentStatus.Active;
    }
} 