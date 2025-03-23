using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using starterkit.Core.Interfaces.Services;
using CoreBlobInfo = starterkit.Core.Modules.Common.Documents.DTOs.BlobInfo;

namespace starterkit.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IBlobStorageService that uses Azure Blob Storage
    /// </summary>
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly ILogger<BlobStorageService> _logger;
        private readonly int _defaultExpiryHours;

        /// <summary>
        /// Initializes a new instance of the BlobStorageService
        /// </summary>
        /// <param name="configuration">Application configuration</param>
        /// <param name="logger">Logger instance</param>
        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"] 
                ?? throw new ArgumentNullException("Azure Blob Storage connection string is not configured");
            _logger = logger;
            _defaultExpiryHours = int.Parse(configuration["AzureBlobStorage:DefaultExpiryHours"] ?? "24");
        }

        /// <inheritdoc />
        public async Task<CoreBlobInfo> UploadAsync(Stream content, string blobName, string contentType, string containerName, bool isPublic)
        {
            try
            {
                // Ensure the container exists
                await EnsureContainerExistsAsync(containerName, isPublic);
                
                // Get a reference to the blob
                var blobClient = GetBlobClient(containerName, blobName);
                
                // Set the content type
                var blobUploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    }
                };
                
                // Upload the content
                await blobClient.UploadAsync(content, blobUploadOptions);
                
                // Return blob info
                return new CoreBlobInfo
                {
                    BlobName = blobName,
                    BlobUrl = blobClient.Uri.ToString(),
                    Size = content.Length,
                    ContainerName = containerName,
                    ContentType = contentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading blob {BlobName} to container {ContainerName}: {Message}", 
                    blobName, containerName, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Stream> DownloadAsync(string blobName, string containerName)
        {
            try
            {
                // Get a reference to the blob
                var blobClient = GetBlobClient(containerName, blobName);
                
                // Check if the blob exists
                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning("Blob {BlobName} not found in container {ContainerName}", 
                        blobName, containerName);
                    return null;
                }
                
                // Download the blob content
                var downloadInfo = await blobClient.DownloadAsync();
                
                // Create a memory stream to hold the content
                var memoryStream = new MemoryStream();
                await downloadInfo.Value.Content.CopyToAsync(memoryStream);
                
                // Reset the position to the beginning
                memoryStream.Position = 0;
                
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading blob {BlobName} from container {ContainerName}: {Message}", 
                    blobName, containerName, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(string blobName, string containerName)
        {
            try
            {
                // Get a reference to the blob
                var blobClient = GetBlobClient(containerName, blobName);
                
                // Delete the blob
                var response = await blobClient.DeleteIfExistsAsync();
                
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blob {BlobName} from container {ContainerName}: {Message}", 
                    blobName, containerName, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetBlobUrlAsync(string blobName, string containerName)
        {
            try
            {
                // Get a reference to the blob
                var blobClient = GetBlobClient(containerName, blobName);
                
                // Check if the blob exists
                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning("Blob {BlobName} not found in container {ContainerName}", 
                        blobName, containerName);
                    return null;
                }
                
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting URL for blob {BlobName} in container {ContainerName}: {Message}", 
                    blobName, containerName, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetSasUrlAsync(string blobName, string containerName, int expiryHours)
        {
            try
            {
                // Get a reference to the blob
                var blobClient = GetBlobClient(containerName, blobName);
                
                // Check if the blob exists
                if (!await blobClient.ExistsAsync())
                {
                    _logger.LogWarning("Blob {BlobName} not found in container {ContainerName}", 
                        blobName, containerName);
                    return null;
                }
                
                // Create the SAS token
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = blobName,
                    Resource = "b", // "b" for blob
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(expiryHours)
                };
                
                // Set permissions
                sasBuilder.SetPermissions(BlobSasPermissions.Read);
                
                // Generate the SAS token
                var sasToken = blobClient.GenerateSasUri(sasBuilder);
                
                return sasToken.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating SAS URL for blob {BlobName} in container {ContainerName}: {Message}", 
                    blobName, containerName, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DoesBlobExistAsync(string blobName, string containerName)
        {
            try
            {
                // Get a reference to the blob
                var blobClient = GetBlobClient(containerName, blobName);
                
                // Check if the blob exists
                return await blobClient.ExistsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if blob {BlobName} exists in container {ContainerName}: {Message}", 
                    blobName, containerName, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> EnsureContainerExistsAsync(string containerName, bool isPublic)
        {
            try
            {
                // Get a reference to the container
                var containerClient = new BlobContainerClient(_connectionString, containerName);
                
                // Create the container if it doesn't exist
                var response = await containerClient.CreateIfNotExistsAsync(
                    isPublic ? PublicAccessType.Blob : PublicAccessType.None);
                
                return response != null && response.Value != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ensuring container {ContainerName} exists: {Message}", 
                    containerName, ex.Message);
                throw;
            }
        }
        
        /// <summary>
        /// Gets a BlobClient instance for the specified container and blob name
        /// </summary>
        /// <param name="containerName">Name of the container</param>
        /// <param name="blobName">Name of the blob</param>
        /// <returns>BlobClient instance</returns>
        private BlobClient GetBlobClient(string containerName, string blobName)
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName);
            return containerClient.GetBlobClient(blobName);
        }
    }
} 