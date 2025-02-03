using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

using starterkit.Core.Modules.Common;

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
        Task<ApiResponse<IEnumerable<Block>>> GetAllAsync();

        /// <summary>
        /// Retrieves all blocks for a specific society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <returns>ApiResponse containing a collection of blocks.</returns>
        Task<ApiResponse<IEnumerable<Block>>> GetBySocietyIdAsync(Guid societyId);

        /// <summary>
        /// Retrieves a block by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>ApiResponse containing the block if found.</returns>
        Task<ApiResponse<Block>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new block.
        /// </summary>
        /// <param name="block">The block entity to create.</param>
        /// <returns>ApiResponse containing the created block.</returns>
        Task<ApiResponse<Block>> CreateAsync(Block block);

        /// <summary>
        /// Updates an existing block.
        /// </summary>
        /// <param name="block">The block entity to update.</param>
        /// <returns>ApiResponse containing the updated block.</returns>
        Task<ApiResponse<Block>> UpdateAsync(Block block);

        /// <summary>
        /// Deletes a block by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the block to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a block exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>ApiResponse indicating if the block exists.</returns>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a block name is unique within a society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <param name="name">The block name to check.</param>
        /// <param name="excludeId">Optional block ID to exclude from the check (for updates).</param>
        /// <returns>ApiResponse indicating if the name is unique.</returns>
        Task<ApiResponse<bool>> IsNameUniqueInSocietyAsync(Guid societyId, string name, Guid? excludeId = null);

        /// <summary>
        /// Retrieves a block with all its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>ApiResponse containing the block with all related entities.</returns>
        Task<ApiResponse<Block>> GetByIdWithDetailsAsync(Guid id);
    }
} 