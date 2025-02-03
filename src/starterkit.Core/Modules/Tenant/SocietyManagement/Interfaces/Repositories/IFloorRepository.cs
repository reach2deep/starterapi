using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing Floor entities.
    /// Provides data access operations for floors within a block.
    /// </summary>
    public interface IFloorRepository
    {
        /// <summary>
        /// Retrieves all floors asynchronously.
        /// </summary>
        /// <returns>A collection of all floors.</returns>
        Task<IEnumerable<Floor>> GetAllAsync();

        /// <summary>
        /// Retrieves all floors for a specific block.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <returns>A collection of floors belonging to the specified block.</returns>
        Task<IEnumerable<Floor>> GetByBlockIdAsync(Guid blockId);

        /// <summary>
        /// Retrieves a floor by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the floor.</param>
        /// <returns>The floor if found; null otherwise.</returns>
        Task<Floor> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds a new floor to the database.
        /// </summary>
        /// <param name="floor">The floor entity to add.</param>
        /// <returns>The added floor with generated ID.</returns>
        Task<Floor> AddAsync(Floor floor);

        /// <summary>
        /// Updates an existing floor in the database.
        /// </summary>
        /// <param name="floor">The floor entity to update.</param>
        /// <returns>The updated floor.</returns>
        Task<Floor> UpdateAsync(Floor floor);

        /// <summary>
        /// Checks if a floor exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the floor.</param>
        /// <returns>True if the floor exists; false otherwise.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a floor number is unique within a block.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="floorNumber">The floor number to check.</param>
        /// <param name="excludeId">Optional floor ID to exclude from the check (for updates).</param>
        /// <returns>True if the floor number is unique; false otherwise.</returns>
        Task<bool> IsFloorNumberUniqueInBlockAsync(Guid blockId, int floorNumber, Guid? excludeId = null);

        /// <summary>
        /// Soft deletes a floor by setting IsActive to false.
        /// </summary>
        /// <param name="id">The unique identifier of the floor to delete.</param>
        /// <returns>True if deletion was successful; false if floor was not found.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a floor with its complete object graph including units.
        /// </summary>
        /// <param name="id">The unique identifier of the floor.</param>
        /// <returns>The floor with all related entities if found; null otherwise.</returns>
        Task<Floor> GetByIdWithDetailsAsync(Guid id);
    }
} 