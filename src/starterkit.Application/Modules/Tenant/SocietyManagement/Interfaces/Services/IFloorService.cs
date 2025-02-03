using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing Floor entities
    /// </summary>
    public interface IFloorService
    {
        /// <summary>
        /// Gets all floors
        /// </summary>
        Task<ApiResponse<IEnumerable<FloorResponse>>> GetAllAsync();

        /// <summary>
        /// Gets all floors for a specific block
        /// </summary>
        Task<ApiResponse<IEnumerable<FloorResponse>>> GetByBlockIdAsync(Guid blockId);

        /// <summary>
        /// Gets a floor by ID
        /// </summary>
        Task<ApiResponse<FloorResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new floor
        /// </summary>
        Task<ApiResponse<FloorResponse>> CreateAsync(CreateFloorRequest request);

        /// <summary>
        /// Updates an existing floor
        /// </summary>
        Task<ApiResponse<FloorResponse>> UpdateAsync(UpdateFloorRequest request);

        /// <summary>
        /// Deletes a floor
        /// </summary>
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
        Task<ApiResponse<FloorResponse>> GetByIdWithDetailsAsync(Guid id);
    }
} 