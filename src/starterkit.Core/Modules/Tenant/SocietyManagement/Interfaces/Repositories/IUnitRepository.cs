using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing Unit entities.
    /// Provides data access operations for residential/commercial units within a floor.
    /// </summary>
    public interface IUnitRepository
    {
        /// <summary>
        /// Retrieves all units asynchronously.
        /// </summary>
        /// <returns>A collection of all units.</returns>
        Task<IEnumerable<Unit>> GetAllAsync();

        /// <summary>
        /// Retrieves all units for a specific floor.
        /// </summary>
        /// <param name="floorId">The ID of the floor.</param>
        /// <returns>A collection of units belonging to the specified floor.</returns>
        Task<IEnumerable<Unit>> GetByFloorIdAsync(Guid floorId);

        /// <summary>
        /// Retrieves all units for a specific block.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <returns>A collection of units belonging to the specified block.</returns>
        Task<IEnumerable<Unit>> GetByBlockIdAsync(Guid blockId);

        /// <summary>
        /// Retrieves all units for a specific society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <returns>A collection of units belonging to the specified society.</returns>
        Task<IEnumerable<Unit>> GetBySocietyIdAsync(Guid societyId);

        /// <summary>
        /// Retrieves a unit by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>The unit if found; null otherwise.</returns>
        Task<Unit> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a unit by its unit number within a society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <param name="unitNumber">The unit number to find.</param>
        /// <returns>The unit if found; null otherwise.</returns>
        Task<Unit> GetByUnitNumberAsync(Guid societyId, string unitNumber);

        /// <summary>
        /// Adds a new unit to the database.
        /// </summary>
        /// <param name="unit">The unit entity to add.</param>
        /// <returns>The added unit with generated ID.</returns>
        Task<Unit> AddAsync(Unit unit);

        /// <summary>
        /// Updates an existing unit in the database.
        /// </summary>
        /// <param name="unit">The unit entity to update.</param>
        /// <returns>The updated unit.</returns>
        Task<Unit> UpdateAsync(Unit unit);

        /// <summary>
        /// Checks if a unit exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>True if the unit exists; false otherwise.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a unit number is unique within a society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <param name="unitNumber">The unit number to check.</param>
        /// <param name="excludeId">Optional unit ID to exclude from the check (for updates).</param>
        /// <returns>True if the unit number is unique; false otherwise.</returns>
        Task<bool> IsUnitNumberUniqueInSocietyAsync(Guid societyId, string unitNumber, Guid? excludeId = null);

        /// <summary>
        /// Soft deletes a unit by setting IsActive to false.
        /// </summary>
        /// <param name="id">The unique identifier of the unit to delete.</param>
        /// <returns>True if deletion was successful; false if unit was not found.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a unit with its complete object graph including
        /// ownership and resident information.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>The unit with all related entities if found; null otherwise.</returns>
        Task<Unit> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Gets a paged list of units
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A tuple containing the paged units and total count.</returns>
        Task<(IEnumerable<Unit> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);
    }
} 