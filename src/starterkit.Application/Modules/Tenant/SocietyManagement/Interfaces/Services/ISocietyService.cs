using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing Society entities.
    /// Provides business logic operations for societies.
    /// </summary>
    public interface ISocietyService
    {
        /// <summary>
        /// Retrieves all societies.
        /// </summary>
        /// <returns>ApiResponse containing a collection of societies.</returns>
        Task<ApiResponse<IEnumerable<Society>>> GetAllAsync();

        /// <summary>
        /// Retrieves a society by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the society.</param>
        /// <returns>ApiResponse containing the society if found.</returns>
        Task<ApiResponse<Society>> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves a society by its registration number.
        /// </summary>
        /// <param name="registrationNumber">The registration number of the society.</param>
        /// <returns>ApiResponse containing the society if found.</returns>
        Task<ApiResponse<Society>> GetByRegistrationNumberAsync(string registrationNumber);

        /// <summary>
        /// Creates a new society.
        /// </summary>
        /// <param name="society">The society entity to create.</param>
        /// <returns>ApiResponse containing the created society.</returns>
        Task<ApiResponse<Society>> CreateAsync(Society society);

        /// <summary>
        /// Updates an existing society.
        /// </summary>
        /// <param name="society">The society entity to update.</param>
        /// <returns>ApiResponse containing the updated society.</returns>
        Task<ApiResponse<Society>> UpdateAsync(Society society);

        /// <summary>
        /// Deletes a society by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the society to delete.</param>
        /// <returns>ApiResponse indicating success or failure.</returns>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves a society with all its related details.
        /// </summary>
        /// <param name="id">The unique identifier of the society.</param>
        /// <returns>ApiResponse containing the society with all related entities.</returns>
        Task<ApiResponse<Society>> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Retrieves all societies with their related details.
        /// </summary>
        /// <returns>ApiResponse containing a collection of societies with all related entities.</returns>
        Task<ApiResponse<IEnumerable<Society>>> GetAllWithDetailsAsync();

        /// <summary>
        /// Checks if a society exists by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the society.</param>
        /// <returns>ApiResponse indicating if the society exists.</returns>
        Task<ApiResponse<bool>> ExistsAsync(Guid id);

        /// <summary>
        /// Checks if a society exists by its registration number.
        /// </summary>
        /// <param name="registrationNumber">The registration number to check.</param>
        /// <returns>ApiResponse indicating if the society exists.</returns>
        Task<ApiResponse<bool>> ExistsByRegistrationNumberAsync(string registrationNumber);
    }
} 