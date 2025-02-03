using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing UnitOwnership entities.
    /// Provides data access operations for unit ownership records.
    /// </summary>
    public interface IUnitOwnershipRepository
    {
        /// <summary>
        /// Retrieves all unit ownership records asynchronously.
        /// </summary>
        /// <returns>A collection of all unit ownership records.</returns>
        Task<IEnumerable<UnitOwnership>> GetAllAsync();

        /// <summary>
        /// Retrieves all ownership records for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>A collection of ownership records for the specified unit.</returns>
        Task<IEnumerable<UnitOwnership>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Retrieves all ownership records for a specific owner.
        /// </summary>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <returns>A collection of ownership records for the specified owner.</returns>
        Task<IEnumerable<UnitOwnership>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Retrieves current ownership record for a specific unit.
        /// </summary>
        /// <param name="unitId">The ID of the unit.</param>
        /// <returns>The current ownership record if found; null otherwise.</returns>
        Task<UnitOwnership> GetCurrentOwnershipAsync(Guid unitId);

        /// <summary>
        /// Retrieves a unit ownership record by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record.</param>
        /// <returns>The ownership record if found; null otherwise.</returns>
        Task<UnitOwnership> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a paged list of ownership records
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A tuple containing the paged ownership records and total count.</returns>
        Task<(IEnumerable<UnitOwnership> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new unit ownership record to the database.
        /// </summary>
        /// <param name="ownership">The ownership record to add.</param>
        /// <returns>The added ownership record with generated ID.</returns>
        Task<UnitOwnership> AddAsync(UnitOwnership ownership);

        /// <summary>
        /// Updates an existing unit ownership record in the database.
        /// </summary>
        /// <param name="ownership">The ownership record to update.</param>
        /// <returns>The updated ownership record.</returns>
        Task<UnitOwnership> UpdateAsync(UnitOwnership ownership);

        /// <summary>
        /// Checks if a unit ownership record exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record.</param>
        /// <returns>True if the ownership record exists; false otherwise.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a unit has any active ownership records.
        /// </summary>
        /// <param name="unitId">The ID of the unit to check.</param>
        /// <returns>True if the unit has active ownership; false otherwise.</returns>
        Task<bool> HasActiveOwnershipAsync(Guid unitId);

        /// <summary>
        /// Soft deletes a unit ownership record by setting IsActive to false.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record to delete.</param>
        /// <returns>True if deletion was successful; false if record was not found.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a unit ownership record with its complete object graph including
        /// unit and owner information.
        /// </summary>
        /// <param name="id">The unique identifier of the ownership record.</param>
        /// <returns>The ownership record with all related entities if found; null otherwise.</returns>
        Task<UnitOwnership> GetByIdWithDetailsAsync(Guid id);
    }
} 