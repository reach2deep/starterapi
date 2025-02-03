using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing UnitResident entities.
    /// Provides business logic operations for unit resident records.
    /// </summary>
    public interface IUnitResidentService
    {
        /// <summary>
        /// Retrieves all unit resident records.
        /// </summary>
        /// <returns>ApiResponse containing a collection of unit resident records.</returns>
        Task<ApiResponse<IEnumerable<UnitResident>>> GetAllAsync();

        /// <summary>
        /// Retrieves all resident records for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>ApiResponse containing a collection of resident records.</returns>
        Task<ApiResponse<IEnumerable<UnitResident>>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Retrieves all units where a specific user is a resident.
        /// </summary>
        /// <param name="residentId">The ID of the resident.</param>
        /// <returns>ApiResponse containing a collection of resident records.</returns>
        Task<ApiResponse<IEnumerable<UnitResident>>> GetByResidentIdAsync(Guid residentId);

        /// <summary>
        /// Retrieves current resident records for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>ApiResponse containing the collection of current resident records.</returns>
        Task<ApiResponse<IEnumerable<UnitResident>>> GetCurrentResidentsAsync(Guid unitId);

        /// <summary>
        /// Retrieves the primary resident record for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>ApiResponse containing the primary resident record if found.</returns>
        Task<ApiResponse<UnitResident>> GetPrimaryResidentAsync(Guid unitId);

        /// <summary>
        /// Retrieves a unit resident record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record.</param>
        /// <returns>ApiResponse containing the resident record if found.</returns>
        Task<ApiResponse<UnitResident>> GetByIdAsync(Guid id);

        /// <summary>
        /// Creates a new unit resident record.
        /// </summary>
        /// <param name="resident">The resident record to create.</param>
        /// <returns>ApiResponse containing the created resident record.</returns>
        Task<ApiResponse<UnitResident>> CreateAsync(UnitResident resident);

        /// <summary>
        /// Updates an existing unit resident record.
        /// </summary>
        /// <param name="resident">The resident record to update.</param>
        /// <returns>ApiResponse containing the updated resident record.</returns>
        Task<ApiResponse<UnitResident>> UpdateAsync(UnitResident resident);

        /// <summary>
        /// Deletes a unit resident record by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a unit resident record exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record.</param>
        /// <returns>ApiResponse indicating if the resident record exists.</returns>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a unit has any active resident records.
        /// </summary>
        /// <param name="unitId">The ID of the unit to check.</param>
        /// <returns>ApiResponse indicating if the unit has active residents.</returns>
        Task<ApiResponse<bool>> HasActiveResidentsAsync(Guid unitId);

        /// <summary>
        /// Checks if a user is currently a resident in any unit.
        /// </summary>
        /// <param name="residentId">The ID of the user to check.</param>
        /// <returns>ApiResponse indicating if the user is an active resident.</returns>
        Task<ApiResponse<bool>> IsActiveResidentAsync(Guid residentId);

        /// <summary>
        /// Retrieves a unit resident record with all its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record.</param>
        /// <returns>ApiResponse containing the resident record with all related entities.</returns>
        Task<ApiResponse<UnitResident>> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Sets a resident as the primary resident for a unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <param name="residentId">The ID of the resident to set as primary.</param>
        /// <returns>ApiResponse containing the updated resident record.</returns>
        Task<ApiResponse<UnitResident>> SetPrimaryResidentAsync(Guid unitId, Guid residentId);

        /// <summary>
        /// Moves a resident out of a unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <param name="residentId">The ID of the resident moving out.</param>
        /// <param name="moveOutDate">The date when the resident moves out.</param>
        /// <returns>ApiResponse containing the updated resident record.</returns>
        Task<ApiResponse<UnitResident>> MoveOutResidentAsync(Guid unitId, Guid residentId, DateTime moveOutDate);
    }
} 