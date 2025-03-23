using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using starterkit.API.Controllers;
using starterkit.Core.Interfaces.Services;
using starterkit.Core.Modules.Common.Documents.DTOs;
using starterkit.Core.Modules.Common.Documents.Enums;

namespace starterkit.API.Controllers.Modules.V1.Tenant.DocumentManagement
{
    /// <summary>
    /// Controller for document management operations
    /// </summary>
    [ApiController]
    [Route("api/v1/tenant/documents")]
    public class DocumentController : BaseApiController
    {
        private readonly IDocumentService _documentService;

        /// <summary>
        /// Initializes a new instance of the DocumentController
        /// </summary>
        /// <param name="documentService">Document service</param>
        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        /// <summary>
        /// Uploads a document
        /// </summary>
        /// <param name="file">File to upload</param>
        /// <param name="entityId">ID of the entity to associate with the document</param>
        /// <param name="moduleType">Type of module the document belongs to</param>
        /// <param name="isPublic">Whether the document should be publicly accessible</param>
        /// <param name="expiryHours">Number of hours the URL should be valid (for temporary public access)</param>
        /// <returns>Uploaded document information</returns>
        [HttpPost]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument(
            [FromForm] IFormFile file,
            [FromForm] Guid entityId,
            [FromForm] ModuleType moduleType,
            [FromForm] bool isPublic = false,
            [FromForm] int? expiryHours = null)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file was provided");

            using var stream = file.OpenReadStream();
            var request = new UploadDocumentRequest
            {
                FileContent = stream,
                FileName = file.FileName,
                ContentType = file.ContentType,
                EntityId = entityId,
                ModuleType = moduleType,
                IsPublic = isPublic,
                ExpiryHours = expiryHours
            };

            var result = await _documentService.UploadDocumentAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Gets a document by ID
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>Document information</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DocumentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocument(Guid id)
        {
            var document = await _documentService.GetDocumentAsync(id);
            if (document == null)
                return NotFound();

            return Ok(document);
        }

        /// <summary>
        /// Gets a URL to download a document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>URL to download the document</returns>
        [HttpGet("{id}/url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocumentUrl(Guid id)
        {
            var url = await _documentService.GetDocumentUrlAsync(id);
            if (url == null)
                return NotFound();

            return Ok(new { Url = url });
        }

        /// <summary>
        /// Gets a temporary URL with specified expiry time
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <param name="expiryHours">Number of hours the URL should be valid</param>
        /// <returns>Temporary URL with specified expiry</returns>
        [HttpGet("{id}/temporary-url")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTemporaryUrl(Guid id, [FromQuery] int expiryHours = 24)
        {
            var url = await _documentService.GetTemporaryUrlAsync(id, expiryHours);
            if (url == null)
                return NotFound();

            return Ok(new { Url = url });
        }

        /// <summary>
        /// Deletes a document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(Guid id)
        {
            var result = await _documentService.DeleteDocumentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Gets all documents for a specific entity
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <param name="moduleType">Module type</param>
        /// <returns>Collection of documents</returns>
        [HttpGet("entity/{entityId}")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentsByEntity(Guid entityId, [FromQuery] ModuleType moduleType)
        {
            var documents = await _documentService.GetDocumentsByEntityAsync(entityId, moduleType);
            return Ok(documents);
        }

        /// <summary>
        /// Gets all documents for a specific module
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <returns>Collection of documents</returns>
        [HttpGet("module/{moduleType}")]
        [ProducesResponseType(typeof(IEnumerable<DocumentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocumentsByModule(ModuleType moduleType)
        {
            var documents = await _documentService.GetDocumentsByModuleAsync(moduleType);
            return Ok(documents);
        }

        /// <summary>
        /// Downloads a document
        /// </summary>
        /// <param name="id">Document ID</param>
        /// <returns>File stream</returns>
        [HttpGet("{id}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadDocument(Guid id)
        {
            var document = await _documentService.GetDocumentAsync(id);
            if (document == null)
                return NotFound();

            var stream = await _documentService.DownloadDocumentAsync(id);
            if (stream == null)
                return NotFound();

            return File(stream, document.ContentType, document.Name);
        }
    }
} 