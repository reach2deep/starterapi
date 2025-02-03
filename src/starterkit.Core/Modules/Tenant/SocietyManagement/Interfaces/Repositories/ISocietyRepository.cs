using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing Society entities.
    /// Provides data access operations for societies within a tenant.
    /// </summary>
    public interface ISocietyRepository
    {
        /// <summary>
        /// Retrieves all societies asynchronously.
        /// </summary>
        /// <returns>A collection of all societies.</returns>
        Task<IEnumerable<Society>> GetAllAsync();

        /// <summary>
        /// Retrieves a society by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the society.</param>
        /// <returns>The society if found; null otherwise.</returns>
        Task<Society> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a society by its registration number.
        /// </summary>
        /// <param name="registrationNumber">The official registration number of the society.</param>
        /// <returns>The society if found; null otherwise.</returns>
        Task<Society> GetByRegistrationNumberAsync(string registrationNumber);

        /// <summary>
        /// Adds a new society to the database.
        /// </summary>
        /// <param name="society">The society entity to add.</param>
        /// <returns>The added society with generated ID.</returns>
        Task<Society> AddAsync(Society society);

        /// <summary>
        /// Updates an existing society in the database.
        /// </summary>
        /// <param name="society">The society entity to update.</param>
        /// <returns>The updated society.</returns>
        Task<Society> UpdateAsync(Society society);

        /// <summary>
        /// Checks if a society exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the society.</param>
        /// <returns>True if the society exists; false otherwise.</returns>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a society exists by its registration number.
        /// </summary>
        /// <param name="registrationNumber">The registration number to check.</param>
        /// <returns>True if a society with the registration number exists; false otherwise.</returns>
        Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber);

        /// <summary>
        /// Soft deletes a society by setting IsActive to false.
        /// </summary>
        /// <param name="id">The unique identifier of the society to delete.</param>
        /// <returns>True if deletion was successful; false if society was not found.</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves societies with their complete object graph including
        /// address, blocks, and subscriptions.
        /// </summary>
        /// <returns>A collection of societies with all related entities.</returns>
        Task<IEnumerable<Society>> GetAllWithDetailsAsync();

        /// <summary>
        /// Retrieves a society with its complete object graph by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the society.</param>
        /// <returns>The society with all related entities if found; null otherwise.</returns>
        Task<Society> GetByIdWithDetailsAsync(Guid id);
    }
} 