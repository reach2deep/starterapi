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

    }
} 