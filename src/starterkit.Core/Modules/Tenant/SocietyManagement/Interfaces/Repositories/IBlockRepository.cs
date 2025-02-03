using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing Block entities.
    /// Provides data access operations for blocks within a society.
    /// </summary>
    public interface IBlockRepository
    {
        /// <summary>
        /// Retrieves all blocks asynchronously.
        /// </summary>
        /// <returns>A collection of all blocks.</returns>
        Task<IEnumerable<Block>> GetAllAsync();

        /// <summary>
        /// Retrieves all blocks for a specific society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <returns>A collection of blocks belonging to the specified society.</returns>
        Task<IEnumerable<Block>> GetBySocietyIdAsync(Guid societyId);

        /// <summary>
        /// Retrieves a block by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>The block if found; null otherwise.</returns>
        Task<Block> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new block to the database.
        /// </summary>
        /// <param name="block">The block entity to add.</param>
        /// <returns>The added block with generated ID.</returns>
        Task<Block> AddAsync(Block block);

        /// <summary>
        /// Updates an existing block in the database.
        /// </summary>
        /// <param name="block">The block entity to update.</param>
        /// <returns>The updated block.</returns>
        Task<Block> UpdateAsync(Block block);

        /// <summary>
        /// Checks if a block exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>True if the block exists; false otherwise.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a block name is unique within a society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <param name="name">The block name to check.</param>
        /// <param name="excludeId">Optional block ID to exclude from the check (for updates).</param>
        /// <returns>True if the name is unique; false otherwise.</returns>
        Task<bool> IsNameUniqueInSocietyAsync(Guid societyId, string name, Guid? excludeId = null);

        /// <summary>
        /// Soft deletes a block by setting IsActive to false.
        /// </summary>
        /// <param name="id">The unique identifier of the block to delete.</param>
        /// <returns>True if deletion was successful; false if block was not found.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a block with its complete object graph including
        /// floors and units.
        /// </summary>
        /// <param name="id">The unique identifier of the block.</param>
        /// <returns>The block with all related entities if found; null otherwise.</returns>
        Task<Block> GetByIdWithDetailsAsync(Guid id);
    }
} 