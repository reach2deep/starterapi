using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing UnitResident entities.
    /// Provides data access operations for unit resident records.
    /// </summary>
    public interface IUnitResidentRepository
    {
        /// <summary>
        /// Retrieves all unit resident records asynchronously.
        /// </summary>
        /// <returns>A collection of all unit resident records.</returns>
        Task<IEnumerable<UnitResident>> GetAllAsync();

        /// <summary>
        /// Retrieves all resident records for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>A collection of resident records for the specified unit.</returns>
        Task<IEnumerable<UnitResident>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Retrieves all units where a specific user is a resident.
        /// </summary>
        /// <param name="residentId">The ID of the resident.</param>
        /// <returns>A collection of resident records for the specified user.</returns>
        Task<IEnumerable<UnitResident>> GetByResidentIdAsync(Guid residentId);

        /// <summary>
        /// Retrieves current resident records for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>Collection of current resident records.</returns>
        Task<IEnumerable<UnitResident>> GetCurrentResidentsAsync(Guid unitId);

        /// <summary>
        /// Retrieves the primary resident record for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>The primary resident record if found; null otherwise.</returns>
        Task<UnitResident> GetPrimaryResidentAsync(Guid unitId);

        /// <summary>
        /// Retrieves a unit resident record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record.</param>
        /// <returns>The resident record if found; null otherwise.</returns>
        Task<UnitResident> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a paged list of resident records
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A tuple containing the paged resident records and total count.</returns>
        Task<(IEnumerable<UnitResident> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new unit resident record to the database.
        /// </summary>
        /// <param name="resident">The resident record to add.</param>
        /// <returns>The added resident record with generated ID.</returns>
        Task<UnitResident> AddAsync(UnitResident resident);

        /// <summary>
        /// Updates an existing unit resident record in the database.
        /// </summary>
        /// <param name="resident">The resident record to update.</param>
        /// <returns>The updated resident record.</returns>
        Task<UnitResident> UpdateAsync(UnitResident resident);

        /// <summary>
        /// Checks if a unit resident record exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record.</param>
        /// <returns>True if the resident record exists; false otherwise.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a unit has any active resident records.
        /// </summary>
        /// <param name="unitId">The ID of the unit to check.</param>
        /// <returns>True if the unit has active residents; false otherwise.</returns>
        Task<bool> HasActiveResidentsAsync(Guid unitId);

        /// <summary>
        /// Checks if a user is currently a resident in any unit.
        /// </summary>
        /// <param name="residentId">The ID of the user to check.</param>
        /// <returns>True if the user is an active resident; false otherwise.</returns>
        Task<bool> IsActiveResidentAsync(Guid residentId);

        /// <summary>
        /// Soft deletes a unit resident record by setting IsActive to false.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record to delete.</param>
        /// <returns>True if deletion was successful; false if record was not found.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a unit resident record with its complete object graph including
        /// unit and resident information.
        /// </summary>
        /// <param name="id">The unique identifier of the resident record.</param>
        /// <returns>The resident record with all related entities if found; null otherwise.</returns>
        Task<UnitResident> GetByIdWithDetailsAsync(Guid id);
    }
} 