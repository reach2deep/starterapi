using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing unit resident records
    /// </summary>
    public interface IUnitResidentService
    {
        /// <summary>
        /// Gets all unit resident records
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetAllAsync();

        /// <summary>
        /// Gets all resident records for a specific unit
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Gets all units where a specific user is a resident
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetByResidentIdAsync(Guid residentId);

        /// <summary>
        /// Gets current resident records for a specific unit
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetCurrentResidentsAsync(Guid unitId);

        /// <summary>
        /// Gets the primary resident record for a specific unit
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> GetPrimaryResidentAsync(Guid unitId);

        /// <summary>
        /// Gets a unit resident record by ID
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new unit resident record
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> CreateAsync(CreateUnitResidentRequest request);

        /// <summary>
        /// Updates an existing unit resident record
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> UpdateAsync(UpdateUnitResidentRequest request);

        /// <summary>
        /// Deletes a unit resident record
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a unit resident record exists
        /// </summary>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a user is currently a resident in any unit
        /// </summary>
        Task<ApiResponse<bool>> IsActiveResidentAsync(Guid residentId);

        /// <summary>
        /// Gets a unit resident record with all its related details
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Sets a resident as the primary resident for a unit
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> SetPrimaryResidentAsync(Guid unitId, Guid residentId);

        /// <summary>
        /// Moves a resident out of a unit
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> MoveOutResidentAsync(Guid unitId, Guid residentId, DateTime moveOutDate);
    }
} 