using System;
using System.IO;
using starterkit.Core.Modules.Common.Documents.Enums;

namespace starterkit.Core.Modules.Common.Documents.DTOs
{
    /// <summary>
    /// Data transfer object for document information
    /// </summary>
    public class DocumentDto
    {
        /// <summary>
        /// Unique identifier of the document
        /// </summary>
        public Guid Id { get; set; }
        
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
        /// URL to access the document
        /// </summary>
        public string Url { get; set; }
        
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
        public DateTime CreatedAt { get; set; }
        
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
        public DocumentStatus Status { get; set; }
    }
    
    /// <summary>
    /// Request object for uploading a document
    /// </summary>
    public class UploadDocumentRequest
    {
        /// <summary>
        /// Content stream of the file
        /// </summary>
        public Stream FileContent { get; set; }
        
        /// <summary>
        /// Original filename
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// MIME type of the file
        /// </summary>
        public string ContentType { get; set; }
        
        /// <summary>
        /// ID of the entity to associate with the document
        /// </summary>
        public Guid EntityId { get; set; }
        
        /// <summary>
        /// Type of module the document belongs to
        /// </summary>
        public ModuleType ModuleType { get; set; }
        
        /// <summary>
        /// Indicates if the document should be publicly accessible
        /// </summary>
        public bool IsPublic { get; set; } = false;
        
        /// <summary>
        /// Number of hours the URL should be valid (for temporary public access)
        /// </summary>
        public int? ExpiryHours { get; set; }
    }
    
    /// <summary>
    /// Information about a blob in Azure Blob Storage
    /// </summary>
    public class BlobInfo
    {
        /// <summary>
        /// Name of the blob in the container
        /// </summary>
        public string BlobName { get; set; }
        
        /// <summary>
        /// URL to access the blob
        /// </summary>
        public string BlobUrl { get; set; }
        
        /// <summary>
        /// Size of the blob in bytes
        /// </summary>
        public long Size { get; set; }
        
        /// <summary>
        /// Name of the container containing the blob
        /// </summary>
        public string ContainerName { get; set; }
        
        /// <summary>
        /// MIME type of the blob
        /// </summary>
        public string ContentType { get; set; }
    }
} 