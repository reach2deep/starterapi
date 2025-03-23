using System;
using System.IO;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common.Documents.DTOs;

namespace starterkit.Core.Interfaces.Services
{
    /// <summary>
    /// Service for interacting with Azure Blob Storage
    /// </summary>
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads content to Azure Blob Storage
        /// </summary>
        /// <param name="content">Content stream to upload</param>
        /// <param name="blobName">Name to assign to the blob</param>
        /// <param name="contentType">MIME type of the content</param>
        /// <param name="containerName">Name of the container to store the blob in</param>
        /// <param name="isPublic">Whether the blob should be publicly accessible</param>
        /// <returns>Information about the uploaded blob</returns>
        Task<BlobInfo> UploadAsync(Stream content, string blobName, string contentType, string containerName, bool isPublic);
        
        /// <summary>
        /// Downloads blob content from Azure Blob Storage
        /// </summary>
        /// <param name="blobName">Name of the blob to download</param>
        /// <param name="containerName">Name of the container containing the blob</param>
        /// <returns>Stream containing the blob content</returns>
        Task<Stream> DownloadAsync(string blobName, string containerName);
        
        /// <summary>
        /// Deletes a blob from Azure Blob Storage
        /// </summary>
        /// <param name="blobName">Name of the blob to delete</param>
        /// <param name="containerName">Name of the container containing the blob</param>
        /// <returns>True if successful, false otherwise</returns>
        Task<bool> DeleteAsync(string blobName, string containerName);
        
        /// <summary>
        /// Gets the URL to access a blob directly
        /// </summary>
        /// <param name="blobName">Name of the blob</param>
        /// <param name="containerName">Name of the container containing the blob</param>
        /// <returns>URL to access the blob</returns>
        Task<string> GetBlobUrlAsync(string blobName, string containerName);
        
        /// <summary>
        /// Gets a temporary URL with Shared Access Signature (SAS) with specified expiry time
        /// </summary>
        /// <param name="blobName">Name of the blob</param>
        /// <param name="containerName">Name of the container containing the blob</param>
        /// <param name="expiryHours">Number of hours the SAS URL should be valid</param>
        /// <returns>SAS URL with specified expiry</returns>
        Task<string> GetSasUrlAsync(string blobName, string containerName, int expiryHours);
        
        /// <summary>
        /// Checks if a blob exists in the specified container
        /// </summary>
        /// <param name="blobName">Name of the blob to check</param>
        /// <param name="containerName">Name of the container to check in</param>
        /// <returns>True if the blob exists, false otherwise</returns>
        Task<bool> DoesBlobExistAsync(string blobName, string containerName);
        
        /// <summary>
        /// Ensures that a container exists, creating it if it doesn't
        /// </summary>
        /// <param name="containerName">Name of the container to ensure exists</param>
        /// <param name="isPublic">Whether the container should be publicly accessible</param>
        /// <returns>True if the container was created, false if it already existed</returns>
        Task<bool> EnsureContainerExistsAsync(string containerName, bool isPublic);
    }
} 