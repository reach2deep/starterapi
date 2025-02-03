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
        /// Gets a resident record by ID
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a paged list of resident records
        /// </summary>
        Task<ApiResponse<PagedResponse<UnitResidentResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Gets resident records data optimized for dropdown/lookup controls
        /// </summary>
        Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request);

        /// <summary>
        /// Creates a new resident record
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> CreateAsync(CreateUnitResidentRequest request);

        /// <summary>
        /// Updates an existing resident record
        /// </summary>
        Task<ApiResponse<UnitResidentResponse>> UpdateAsync(UpdateUnitResidentRequest request);

        /// <summary>
        /// Deletes a resident record
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a user is currently a resident in any unit
        /// </summary>
        Task<ApiResponse<bool>> IsActiveResidentAsync(Guid residentId);

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