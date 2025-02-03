using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing Floor entities.
    /// Provides business logic operations for floors within blocks.
    /// </summary>
    public interface IFloorService
    {
        /// <summary>
        /// Retrieves all floors.
        /// </summary>
        /// <returns>ApiResponse containing a collection of floors.</returns>
        Task<ApiResponse<IEnumerable<Floor>>> GetAllAsync();

        /// <summary>
        /// Retrieves all floors for a specific block.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <returns>ApiResponse containing a collection of floors.</returns>
        Task<ApiResponse<IEnumerable<Floor>>> GetByBlockIdAsync(Guid blockId);

        /// <summary>
        /// Retrieves a floor by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the floor.</param>
        /// <returns>ApiResponse containing the floor if found.</returns>
        Task<ApiResponse<Floor>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new floor.
        /// </summary>
        /// <param name="floor">The floor entity to create.</param>
        /// <returns>ApiResponse containing the created floor.</returns>
        Task<ApiResponse<Floor>> CreateAsync(Floor floor);

        /// <summary>
        /// Updates an existing floor.
        /// </summary>
        /// <param name="floor">The floor entity to update.</param>
        /// <returns>ApiResponse containing the updated floor.</returns>
        Task<ApiResponse<Floor>> UpdateAsync(Floor floor);

        /// <summary>
        /// Deletes a floor by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the floor to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a floor exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the floor.</param>
        /// <returns>ApiResponse indicating if the floor exists.</returns>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a floor number is unique within a block.
        /// </summary>
        /// <param name="blockId">The ID of the block.</param>
        /// <param name="floorNumber">The floor number to check.</param>
        /// <param name="excludeId">Optional floor ID to exclude from the check (for updates).</param>
        /// <returns>ApiResponse indicating if the floor number is unique.</returns>
        Task<ApiResponse<bool>> IsFloorNumberUniqueInBlockAsync(Guid blockId, int floorNumber, Guid? excludeId = null);

        /// <summary>
        /// Retrieves a floor with all its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the floor.</param>
        /// <returns>ApiResponse containing the floor with all related entities.</returns>
        Task<ApiResponse<Floor>> GetByIdWithDetailsAsync(Guid id);
    }
} 