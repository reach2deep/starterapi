using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common.Documents.DTOs;
using starterkit.Core.Modules.Common.Documents.Enums;

namespace starterkit.Core.Interfaces.Services
{
    /// <summary>
    /// Service for managing documents in the system
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Uploads a document to storage
        /// </summary>
        /// <param name="request">Document upload request with file data and metadata</param>
        /// <returns>The uploaded document information</returns>
        Task<DocumentDto> UploadDocumentAsync(UploadDocumentRequest request);
        
        /// <summary>
        /// Gets a document by its ID
        /// </summary>
        /// <param name="documentId">Unique identifier of the document</param>
        /// <returns>The document information</returns>
        Task<DocumentDto> GetDocumentAsync(Guid documentId);
        
        /// <summary>
        /// Gets a URL to access the document directly
        /// </summary>
        /// <param name="documentId">Unique identifier of the document</param>
        /// <returns>URL to access the document</returns>
        Task<string> GetDocumentUrlAsync(Guid documentId);
        
        /// <summary>
        /// Gets a temporary URL with specified expiry time
        /// </summary>
        /// <param name="documentId">Unique identifier of the document</param>
        /// <param name="expiryHours">Number of hours the URL should be valid</param>
        /// <returns>Temporary URL with specified expiry</returns>
        Task<string> GetTemporaryUrlAsync(Guid documentId, int expiryHours);
        
        /// <summary>
        /// Deletes a document (soft delete)
        /// </summary>
        /// <param name="documentId">Unique identifier of the document</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteDocumentAsync(Guid documentId);
        
        /// <summary>
        /// Gets all documents associated with a specific entity
        /// </summary>
        /// <param name="entityId">Unique identifier of the entity</param>
        /// <param name="moduleType">Type of module the entity belongs to</param>
        /// <returns>Collection of documents associated with the entity</returns>
        Task<IEnumerable<DocumentDto>> GetDocumentsByEntityAsync(Guid entityId, ModuleType moduleType);
        
        /// <summary>
        /// Gets all documents for a specific module type
        /// </summary>
        /// <param name="moduleType">Type of module</param>
        /// <returns>Collection of documents for the module</returns>
        Task<IEnumerable<DocumentDto>> GetDocumentsByModuleAsync(ModuleType moduleType);
        
        /// <summary>
        /// Downloads a document as a stream
        /// </summary>
        /// <param name="documentId">Unique identifier of the document</param>
        /// <returns>Stream containing the document content</returns>
        Task<Stream> DownloadDocumentAsync(Guid documentId);
    }
} 