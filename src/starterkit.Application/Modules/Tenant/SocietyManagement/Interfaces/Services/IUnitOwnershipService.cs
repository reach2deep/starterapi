using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing UnitOwnership entities.
    /// Provides business logic operations for unit ownership records.
    /// </summary>
    public interface IUnitOwnershipService
    {
        /// <summary>
        /// Retrieves all unit ownership records.
        /// </summary>
        /// <returns>ApiResponse containing a collection of unit ownership records.</returns>
        Task<ApiResponse<IEnumerable<UnitOwnership>>> GetAllAsync();

        /// <summary>
        /// Retrieves all ownership records for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>ApiResponse containing a collection of ownership records.</returns>
        Task<ApiResponse<IEnumerable<UnitOwnership>>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Retrieves all ownership records for a specific owner.
        /// </summary>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <returns>ApiResponse containing a collection of ownership records.</returns>
        Task<ApiResponse<IEnumerable<UnitOwnership>>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Retrieves current ownership record for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>ApiResponse containing the current ownership record if found.</returns>
        Task<ApiResponse<UnitOwnership>> GetCurrentOwnershipAsync(Guid unitId);

        /// <summary>
        /// Retrieves a unit ownership record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record.</param>
        /// <returns>ApiResponse containing the ownership record if found.</returns>
        Task<ApiResponse<UnitOwnership>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new unit ownership record.
        /// </summary>
        /// <param name="ownership">The ownership record to create.</param>
        /// <returns>ApiResponse containing the created ownership record.</returns>
        Task<ApiResponse<UnitOwnership>> CreateAsync(UnitOwnership ownership);

        /// <summary>
        /// Updates an existing unit ownership record.
        /// </summary>
        /// <param name="ownership">The ownership record to update.</param>
        /// <returns>ApiResponse containing the updated ownership record.</returns>
        Task<ApiResponse<UnitOwnership>> UpdateAsync(UnitOwnership ownership);

        /// <summary>
        /// Deletes a unit ownership record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a unit ownership record exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record.</param>
        /// <returns>ApiResponse indicating if the ownership record exists.</returns>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a unit has any active ownership records.
        /// </summary>
        /// <param name="unitId">The ID of the unit to check.</param>
        /// <returns>ApiResponse indicating if the unit has active ownership.</returns>
        Task<ApiResponse<bool>> HasActiveOwnershipAsync(Guid unitId);

        /// <summary>
        /// Retrieves a unit ownership record with all its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record.</param>
        /// <returns>ApiResponse containing the ownership record with all related entities.</returns>
        Task<ApiResponse<UnitOwnership>> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Transfers ownership of a unit from one owner to another.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <param name="currentOwnerId">The ID of the current owner.</param>
        /// <param name="newOwnerId">The ID of the new owner.</param>
        /// <param name="transferDate">The date when the transfer takes effect.</param>
        /// <returns>ApiResponse containing the new ownership record.</returns>
        Task<ApiResponse<UnitOwnership>> TransferOwnershipAsync(Guid unitId, Guid currentOwnerId, Guid newOwnerId, DateTime transferDate);
    }
} 