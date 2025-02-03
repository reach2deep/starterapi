using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing Block entities.
    /// Provides business logic operations for blocks within societies.
    /// </summary>
    public interface IBlockService
    {
        /// <summary>
        /// Retrieves all blocks.
        /// </summary>
        /// <returns>ApiResponse containing a collection of blocks.</returns>
        Task<ApiResponse<IEnumerable<BlockResponse>>> GetAllAsync();

        
        /// <summary>
        /// Retrieves a block by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>ApiResponse containing the block if found.</returns>
        Task<ApiResponse<BlockResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a paged list of blocks
        /// </summary>
        Task<ApiResponse<PagedResponse<BlockResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Gets blocks data optimized for dropdown/lookup controls
        /// </summary>
        Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request);

        /// <summary>
        /// Creates a new block.
        /// </summary>
        /// <param name="request">The block creation request.</param>
        /// <returns>ApiResponse containing the created block.</returns>
        Task<ApiResponse<BlockResponse>> CreateAsync(CreateBlockRequest request);

        /// <summary>
        /// Updates an existing block.
        /// </summary>
        /// <param name="request">The block update request.</param>
        /// <returns>ApiResponse containing the updated block.</returns>
        Task<ApiResponse<BlockResponse>> UpdateAsync(UpdateBlockRequest request);

        /// <summary>
        /// Deletes a block by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the block to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

    }
} 