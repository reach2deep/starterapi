using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Implementation of ISocietyService.
    /// Provides business logic operations for managing societies.
    /// </summary>
    public class SocietyService : ISocietyService
    {
        private readonly ISocietyRepository _societyRepository;
        private readonly ILogger<SocietyService> _logger;

        /// <summary>
        /// Initializes a new instance of the SocietyService class.
        /// </summary>
        /// <param name="societyRepository">The society repository.</param>
        /// <param name="logger">The logger instance.</param>
        public SocietyService(ISocietyRepository societyRepository, ILogger<SocietyService> logger)
        {
            _societyRepository = societyRepository ?? throw new ArgumentNullException(nameof(societyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Society>>> GetAllAsync()
        {
            try
            {
                var societies = await _societyRepository.GetAllAsync();
                return ApiResponse<IEnumerable<Society>>.CreateSuccess(societies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all societies");
                return ApiResponse<IEnumerable<Society>>.CreateError("Failed to retrieve societies");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Society>> GetByIdAsync(Guid id)
        {
            try
            {
                var society = await _societyRepository.GetByIdAsync(id);
                if (society == null)
                    return ApiResponse<Society>.CreateError("Society not found");

                return ApiResponse<Society>.CreateSuccess(society);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society with ID: {Id}", id);
                return ApiResponse<Society>.CreateError("Failed to retrieve society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Society>> GetByRegistrationNumberAsync(string registrationNumber)
        {
            try
            {
                var society = await _societyRepository.GetByRegistrationNumberAsync(registrationNumber);
                if (society == null)
                    return ApiResponse<Society>.CreateError("Society not found");

                return ApiResponse<Society>.CreateSuccess(society);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society with registration number: {RegistrationNumber}", registrationNumber);
                return ApiResponse<Society>.CreateError("Failed to retrieve society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Society>> CreateAsync(Society society)
        {
            try
            {
                if (society == null)
                    return ApiResponse<Society>.CreateError("Society cannot be null");

                if (await _societyRepository.ExistsByRegistrationNumberAsync(society.RegistrationNumber))
                    return ApiResponse<Society>.CreateError("Society with this registration number already exists");

                var createdSociety = await _societyRepository.AddAsync(society);
                return ApiResponse<Society>.CreateSuccess(createdSociety);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating society: {Name}", society?.Name);
                return ApiResponse<Society>.CreateError("Failed to create society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Society>> UpdateAsync(Society society)
        {
            try
            {
                if (society == null)
                    return ApiResponse<Society>.CreateError("Society cannot be null");

                if (!await _societyRepository.ExistsAsync(society.Id))
                    return ApiResponse<Society>.CreateError("Society not found");

                var existingSociety = await _societyRepository.GetByRegistrationNumberAsync(society.RegistrationNumber);
                if (existingSociety != null && existingSociety.Id != society.Id)
                    return ApiResponse<Society>.CreateError("Society with this registration number already exists");

                var updatedSociety = await _societyRepository.UpdateAsync(society);
                return ApiResponse<Society>.CreateSuccess(updatedSociety);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating society with ID: {Id}", society?.Id);
                return ApiResponse<Society>.CreateError("Failed to update society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Society not found");

                var result = await _societyRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting society with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Society>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var society = await _societyRepository.GetByIdWithDetailsAsync(id);
                if (society == null)
                    return ApiResponse<Society>.CreateError("Society not found");

                return ApiResponse<Society>.CreateSuccess(society);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society details with ID: {Id}", id);
                return ApiResponse<Society>.CreateError("Failed to retrieve society details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Society>>> GetAllWithDetailsAsync()
        {
            try
            {
                var societies = await _societyRepository.GetAllWithDetailsAsync();
                return ApiResponse<IEnumerable<Society>>.CreateSuccess(societies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all societies with details");
                return ApiResponse<IEnumerable<Society>>.CreateError("Failed to retrieve societies with details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _societyRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of society with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check society existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsByRegistrationNumberAsync(string registrationNumber)
        {
            try
            {
                var exists = await _societyRepository.ExistsByRegistrationNumberAsync(registrationNumber);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of society with registration number: {RegistrationNumber}", registrationNumber);
                return ApiResponse<bool>.CreateError("Failed to check society existence");
            }
        }
    }
} 