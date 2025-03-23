using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using starterkit.Application.Persistence;
using starterkit.Core.Interfaces.Services;
using starterkit.Core.Modules.Common.Documents.DTOs;
using starterkit.Core.Modules.Common.Documents.Entities;
using starterkit.Core.Modules.Common.Documents.Enums;

namespace starterkit.Infrastructure.Services
{
    /// <summary>
    /// Implementation of the document management service
    /// </summary>
    public class DocumentService : IDocumentService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITenantDbContext _dbContext;
        private readonly ILogger<DocumentService> _logger;
        private readonly string _tenantId;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the DocumentService class
        /// </summary>
        /// <param name="blobStorageService">Blob storage service</param>
        /// <param name="dbContext">Database context</param>
        /// <param name="logger">Logger</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="configuration">Configuration</param>
        public DocumentService(
            IBlobStorageService blobStorageService,
            ITenantDbContext dbContext,
            ILogger<DocumentService> logger,
            string tenantId,
            IConfiguration configuration)
        {
            _blobStorageService = blobStorageService;
            _dbContext = dbContext;
            _logger = logger;
            _tenantId = tenantId;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<DocumentDto> UploadDocumentAsync(UploadDocumentRequest request)
        {
            try
            {
                // Validate input
                if (request.FileContent == null)
                    throw new ArgumentNullException(nameof(request.FileContent), "File content is required");
                
                if (string.IsNullOrWhiteSpace(request.FileName))
                    throw new ArgumentNullException(nameof(request.FileName), "File name is required");
                
                if (string.IsNullOrWhiteSpace(request.ContentType))
                    throw new ArgumentNullException(nameof(request.ContentType), "Content type is required");
                
                // Generate a unique blob name using GUID
                var fileExtension = Path.GetExtension(request.FileName);
                var blobName = $"{Guid.NewGuid()}{fileExtension}";
                
                // Determine the container name based on the module type and public/private status
                var containerName = GetContainerName(request.ModuleType, request.IsPublic);
                
                // Upload the blob to Azure Storage
                var blobInfo = await _blobStorageService.UploadAsync(
                    request.FileContent,
                    blobName,
                    request.ContentType,
                    containerName,
                    request.IsPublic);
                
                // Calculate expiry date if specified
                DateTime? expiryDate = null;
                if (request.ExpiryHours.HasValue)
                {
                    expiryDate = DateTime.UtcNow.AddHours(request.ExpiryHours.Value);
                }
                
                // Create the document entity
                var document = new Document
                {
                    Id = Guid.NewGuid(),
                    Name = request.FileName,
                    ContentType = request.ContentType,
                    Size = blobInfo.Size,
                    BlobUrl = blobInfo.BlobUrl,
                    BlobName = blobInfo.BlobName,
                    ContainerName = blobInfo.ContainerName,
                    IsPublic = request.IsPublic,
                    ExpiryDate = expiryDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    TenantId = Guid.Parse(_tenantId),
                    ModuleType = request.ModuleType,
                    EntityId = request.EntityId,
                    Status = DocumentStatus.Active
                };
                
                // Save to database
                _dbContext.Documents.Add(document);
                await _dbContext.SaveChangesAsync();
                
                // Map to DTO and return
                return MapToDto(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document: {Message}", ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<DocumentDto> GetDocumentAsync(Guid documentId)
        {
            try
            {
                // Get the document from the database
                var document = await GetDocumentEntityAsync(documentId);
                if (document == null)
                    return null;
                
                // Map to DTO and return
                return MapToDto(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document {DocumentId}: {Message}", documentId, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetDocumentUrlAsync(Guid documentId)
        {
            try
            {
                // Get the document from the database
                var document = await GetDocumentEntityAsync(documentId);
                if (document == null)
                    return null;
                
                // For public documents, return the blob URL directly
                if (document.IsPublic)
                    return document.BlobUrl;
                
                // For private documents, generate a SAS URL with default expiry
                int defaultExpiryHours = int.Parse(_configuration["AzureBlobStorage:DefaultExpiryHours"] ?? "24");
                return await _blobStorageService.GetSasUrlAsync(
                    document.BlobName, 
                    document.ContainerName, 
                    defaultExpiryHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting document URL for {DocumentId}: {Message}", documentId, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetTemporaryUrlAsync(Guid documentId, int expiryHours)
        {
            try
            {
                // Get the document from the database
                var document = await GetDocumentEntityAsync(documentId);
                if (document == null)
                    return null;
                
                // Generate a SAS URL with the specified expiry
                return await _blobStorageService.GetSasUrlAsync(
                    document.BlobName, 
                    document.ContainerName, 
                    expiryHours);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting temporary URL for {DocumentId}: {Message}", documentId, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteDocumentAsync(Guid documentId)
        {
            try
            {
                // Get the document from the database
                var document = await GetDocumentEntityAsync(documentId);
                if (document == null)
                    return false;
                
                // Soft delete in the database
                document.Status = DocumentStatus.Deleted;
                document.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                
                // Optionally, delete from blob storage as well (commented out for soft delete only)
                // await _blobStorageService.DeleteAsync(document.BlobName, document.ContainerName);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId}: {Message}", documentId, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DocumentDto>> GetDocumentsByEntityAsync(Guid entityId, ModuleType moduleType)
        {
            try
            {
                // Get all active documents for the specified entity and module type
                var documents = _dbContext.Documents
                    .Where(d => d.EntityId == entityId && 
                           d.ModuleType == moduleType && 
                           d.Status == DocumentStatus.Active)
                    .ToList();
                
                // Map to DTOs and return
                return documents.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for entity {EntityId} and module {ModuleType}: {Message}", 
                    entityId, moduleType, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DocumentDto>> GetDocumentsByModuleAsync(ModuleType moduleType)
        {
            try
            {
                // Get all active documents for the specified module type
                var documents = _dbContext.Documents
                    .Where(d => d.ModuleType == moduleType && 
                           d.Status == DocumentStatus.Active)
                    .ToList();
                
                // Map to DTOs and return
                return documents.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents for module {ModuleType}: {Message}", 
                    moduleType, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Stream> DownloadDocumentAsync(Guid documentId)
        {
            try
            {
                // Get the document from the database
                var document = await GetDocumentEntityAsync(documentId);
                if (document == null)
                    return null;
                
                // Log access
                await LogDocumentAccessAsync(document, AccessType.Download);
                
                // Download from blob storage
                return await _blobStorageService.DownloadAsync(
                    document.BlobName, 
                    document.ContainerName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {DocumentId}: {Message}", documentId, ex.Message);
                throw;
            }
        }
        
        /// <summary>
        /// Gets a document entity by ID
        /// </summary>
        /// <param name="documentId">Document ID</param>
        /// <returns>Document entity or null if not found</returns>
        private async Task<Document> GetDocumentEntityAsync(Guid documentId)
        {
            return _dbContext.Documents
                .FirstOrDefault(d => d.Id == documentId);
        }
        
        /// <summary>
        /// Maps a Document entity to a DocumentDto
        /// </summary>
        /// <param name="document">Document entity</param>
        /// <returns>DocumentDto</returns>
        private DocumentDto MapToDto(Document document)
        {
            return new DocumentDto
            {
                Id = document.Id,
                Name = document.Name,
                ContentType = document.ContentType,
                Size = document.Size,
                Url = document.IsPublic 
                    ? document.BlobUrl 
                    : null, // URL for private documents is provided on demand
                IsPublic = document.IsPublic,
                ExpiryDate = document.ExpiryDate,
                CreatedAt = document.CreatedAt,
                ModuleType = document.ModuleType,
                EntityId = document.EntityId,
                Status = document.Status
            };
        }
        
        /// <summary>
        /// Logs access to a document
        /// </summary>
        /// <param name="document">Document being accessed</param>
        /// <param name="accessType">Type of access</param>
        /// <returns>Task</returns>
        private async Task LogDocumentAccessAsync(Document document, AccessType accessType)
        {
            try
            {
                // Create an access log entry
                var log = new DocumentAccessLog
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    AccessedById = null, // TODO: Get from current user context
                    IpAddress = "0.0.0.0", // TODO: Get from current request
                    AccessedAt = DateTime.UtcNow,
                    AccessType = accessType
                };
                
                // Save to database
                _dbContext.DocumentAccessLogs.Add(log);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Just log the error but don't fail the operation
                _logger.LogWarning(ex, "Error logging document access: {Message}", ex.Message);
            }
        }
        
        /// <summary>
        /// Gets the container name based on module type and public/private status
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="isPublic">Whether the document is public</param>
        /// <returns>Container name</returns>
        private string GetContainerName(ModuleType moduleType, bool isPublic)
        {
            var tenantPrefix = $"tenant-{_tenantId.ToLower()}";
            var visibility = isPublic ? "public" : "private";
            
            string modulePrefix;
            switch (moduleType)
            {
                case ModuleType.Profile:
                    modulePrefix = "profile-images";
                    break;
                case ModuleType.Block:
                case ModuleType.Unit:
                    modulePrefix = "property-images";
                    break;
                case ModuleType.Community:
                    modulePrefix = "community-files";
                    break;
                default:
                    modulePrefix = "documents";
                    break;
            }
            
            return $"{tenantPrefix}-{modulePrefix}-{visibility}".ToLower();
        }
    }
} 