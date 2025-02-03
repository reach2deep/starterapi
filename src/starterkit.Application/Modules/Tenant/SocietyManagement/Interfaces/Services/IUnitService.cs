using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing Unit entities.
    /// Provides business logic operations for units within floors.
    /// </summary>
    public interface IUnitService
    {
        /// <summary>
        /// Retrieves all units.
        /// </summary>
        /// <returns>ApiResponse containing a collection of units.</returns>
        Task<ApiResponse<IEnumerable<Unit>>> GetAllAsync();

        /// <summary>
        /// Retrieves all units for a specific floor.
        /// </summary>
        /// <param name="floorId">The ID of the floor.</param>
        /// <returns>ApiResponse containing a collection of units.</returns>
        Task<ApiResponse<IEnumerable<Unit>>> GetByFloorIdAsync(Guid floorId);

        /// <summary>
        /// Retrieves all units for a specific block.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <returns>ApiResponse containing a collection of units.</returns>
        Task<ApiResponse<IEnumerable<Unit>>> GetByBlockIdAsync(Guid blockId);

        /// <summary>
        /// Retrieves all units for a specific society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <returns>ApiResponse containing a collection of units.</returns>
        Task<ApiResponse<IEnumerable<Unit>>> GetBySocietyIdAsync(Guid societyId);

        /// <summary>
        /// Retrieves a unit by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>ApiResponse containing the unit if found.</returns>
        Task<ApiResponse<Unit>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a unit by its unit number within a society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <param name="unitNumber">The unit number to find.</param>
        /// <returns>ApiResponse containing the unit if found.</returns>
        Task<ApiResponse<Unit>> GetByUnitNumberAsync(Guid societyId, string unitNumber);

        /// <summary>
        /// Creates a new unit.
        /// </summary>
        /// <param name="unit">The unit entity to create.</param>
        /// <returns>ApiResponse containing the created unit.</returns>
        Task<ApiResponse<Unit>> CreateAsync(Unit unit);

        /// <summary>
        /// Updates an existing unit.
        /// </summary>
        /// <param name="unit">The unit entity to update.</param>
        /// <returns>ApiResponse containing the updated unit.</returns>
        Task<ApiResponse<Unit>> UpdateAsync(Unit unit);

        /// <summary>
        /// Deletes a unit by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the unit to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a unit exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>ApiResponse indicating if the unit exists.</returns>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a unit number is unique within a society.
        /// </summary>
        /// <param name="societyId">The ID of the society.</param>
        /// <param name="unitNumber">The unit number to check.</param>
        /// <param name="excludeId">Optional unit ID to exclude from the check (for updates).</param>
        /// <returns>ApiResponse indicating if the unit number is unique.</returns>
        Task<ApiResponse<bool>> IsUnitNumberUniqueInSocietyAsync(Guid societyId, string unitNumber, Guid? excludeId = null);

        /// <summary>
        /// Retrieves a unit with all its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>ApiResponse containing the unit with all related entities.</returns>
        Task<ApiResponse<Unit>> GetByIdWithDetailsAsync(Guid id);
    }
} 