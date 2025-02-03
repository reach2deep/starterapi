using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing Unit entities
    /// </summary>
    public interface IUnitService
    {
        /// <summary>
        /// Gets all units
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitResponse>>> GetAllAsync();

        /// <summary>
        /// Gets all units for a specific floor
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitResponse>>> GetByFloorIdAsync(Guid floorId);

        /// <summary>
        /// Gets a unit by ID
        /// </summary>
        Task<ApiResponse<UnitResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new unit
        /// </summary>
        Task<ApiResponse<UnitResponse>> CreateAsync(CreateUnitRequest request);

        /// <summary>
        /// Updates an existing unit
        /// </summary>
        Task<ApiResponse<UnitResponse>> UpdateAsync(UpdateUnitRequest request);

        /// <summary>
        /// Deletes a unit
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

     
    }
} 