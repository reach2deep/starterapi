using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing unit ownership records
    /// </summary>
    public interface IUnitOwnershipService
    {
        /// <summary>
        /// Gets all unit ownership records
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitOwnershipResponse>>> GetAllAsync();

        /// <summary>
        /// Gets all ownership records for a specific unit
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitOwnershipResponse>>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Gets all ownership records for a specific owner
        /// </summary>
        Task<ApiResponse<IEnumerable<UnitOwnershipResponse>>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Gets current ownership record for a specific unit
        /// </summary>
        Task<ApiResponse<UnitOwnershipResponse>> GetCurrentOwnershipAsync(Guid unitId);

        /// <summary>
        /// Gets a unit ownership record by ID
        /// </summary>
        Task<ApiResponse<UnitOwnershipResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new unit ownership record
        /// </summary>
        Task<ApiResponse<UnitOwnershipResponse>> CreateAsync(CreateUnitOwnershipRequest request);

        /// <summary>
        /// Updates an existing unit ownership record
        /// </summary>
        Task<ApiResponse<UnitOwnershipResponse>> UpdateAsync(UpdateUnitOwnershipRequest request);

        /// <summary>
        /// Deletes a unit ownership record
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);



        /// <summary>
        /// Transfers ownership of a unit from one owner to another
        /// </summary>
        Task<ApiResponse<UnitOwnershipResponse>> TransferOwnershipAsync(Guid unitId, Guid currentOwnerId, Guid newOwnerId, DateTime transferDate);
    }
} 