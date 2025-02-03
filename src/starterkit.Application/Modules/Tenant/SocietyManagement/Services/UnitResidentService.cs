using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Implementation of IUnitResidentService.
    /// Provides business logic operations for managing unit resident records.
    /// </summary>
    public class UnitResidentService : IUnitResidentService
    {
        private readonly IUnitResidentRepository _residentRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ILogger<UnitResidentService> _logger;

        /// <summary>
        /// Initializes a new instance of the UnitResidentService class.
        /// </summary>
        /// <param name="residentRepository">The unit resident repository.</param>
        /// <param name="unitRepository">The unit repository.</param>
        /// <param name="logger">The logger instance.</param>
        public UnitResidentService(
            IUnitResidentRepository residentRepository,
            IUnitRepository unitRepository,
            ILogger<UnitResidentService> logger)
        {
            _residentRepository = residentRepository ?? throw new ArgumentNullException(nameof(residentRepository));
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResident>>> GetAllAsync()
        {
            try
            {
                var residents = await _residentRepository.GetAllAsync();
                return ApiResponse<IEnumerable<UnitResident>>.CreateSuccess(residents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all unit resident records");
                return ApiResponse<IEnumerable<UnitResident>>.CreateError("Failed to retrieve resident records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResident>>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<IEnumerable<UnitResident>>.CreateError("Unit not found");

                var residents = await _residentRepository.GetByUnitIdAsync(unitId);
                return ApiResponse<IEnumerable<UnitResident>>.CreateSuccess(residents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident records for unit ID: {UnitId}", unitId);
                return ApiResponse<IEnumerable<UnitResident>>.CreateError("Failed to retrieve resident records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResident>>> GetByResidentIdAsync(Guid residentId)
        {
            try
            {
                var residents = await _residentRepository.GetByResidentIdAsync(residentId);
                return ApiResponse<IEnumerable<UnitResident>>.CreateSuccess(residents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident records for resident ID: {ResidentId}", residentId);
                return ApiResponse<IEnumerable<UnitResident>>.CreateError("Failed to retrieve resident records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResident>>> GetCurrentResidentsAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<IEnumerable<UnitResident>>.CreateError("Unit not found");

                var residents = await _residentRepository.GetCurrentResidentsAsync(unitId);
                return ApiResponse<IEnumerable<UnitResident>>.CreateSuccess(residents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current residents for unit ID: {UnitId}", unitId);
                return ApiResponse<IEnumerable<UnitResident>>.CreateError("Failed to retrieve current residents");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> GetPrimaryResidentAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitResident>.CreateError("Unit not found");

                var resident = await _residentRepository.GetPrimaryResidentAsync(unitId);
                if (resident == null)
                    return ApiResponse<UnitResident>.CreateError("No primary resident found");

                return ApiResponse<UnitResident>.CreateSuccess(resident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving primary resident for unit ID: {UnitId}", unitId);
                return ApiResponse<UnitResident>.CreateError("Failed to retrieve primary resident");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> GetByIdAsync(Guid id)
        {
            try
            {
                var resident = await _residentRepository.GetByIdAsync(id);
                if (resident == null)
                    return ApiResponse<UnitResident>.CreateError("Resident record not found");

                return ApiResponse<UnitResident>.CreateSuccess(resident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident record with ID: {Id}", id);
                return ApiResponse<UnitResident>.CreateError("Failed to retrieve resident record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> CreateAsync(UnitResident resident)
        {
            try
            {
                if (resident == null)
                    return ApiResponse<UnitResident>.CreateError("Resident record cannot be null");

                if (!await _unitRepository.ExistsAsync(resident.UnitId))
                    return ApiResponse<UnitResident>.CreateError("Unit not found");

                // If this is marked as primary resident, ensure no other primary resident exists
                if (resident.IsPrimary)
                {
                    var primaryResident = await _residentRepository.GetPrimaryResidentAsync(resident.UnitId);
                    if (primaryResident != null)
                        return ApiResponse<UnitResident>.CreateError("Unit already has a primary resident");
                }

                var createdResident = await _residentRepository.AddAsync(resident);
                return ApiResponse<UnitResident>.CreateSuccess(createdResident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating resident record for unit ID: {UnitId}", resident?.UnitId);
                return ApiResponse<UnitResident>.CreateError("Failed to create resident record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> UpdateAsync(UnitResident resident)
        {
            try
            {
                if (resident == null)
                    return ApiResponse<UnitResident>.CreateError("Resident record cannot be null");

                if (!await _residentRepository.ExistsAsync(resident.Id))
                    return ApiResponse<UnitResident>.CreateError("Resident record not found");

                if (!await _unitRepository.ExistsAsync(resident.UnitId))
                    return ApiResponse<UnitResident>.CreateError("Unit not found");

                // If this is being marked as primary resident, ensure no other primary resident exists
                if (resident.IsPrimary)
                {
                    var primaryResident = await _residentRepository.GetPrimaryResidentAsync(resident.UnitId);
                    if (primaryResident != null && primaryResident.Id != resident.Id)
                        return ApiResponse<UnitResident>.CreateError("Unit already has a primary resident");
                }

                var updatedResident = await _residentRepository.UpdateAsync(resident);
                return ApiResponse<UnitResident>.CreateSuccess(updatedResident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating resident record with ID: {Id}", resident?.Id);
                return ApiResponse<UnitResident>.CreateError("Failed to update resident record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _residentRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Resident record not found");

                var result = await _residentRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting resident record with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete resident record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _residentRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of resident record with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check resident record existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> HasActiveResidentsAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<bool>.CreateError("Unit not found");

                var hasActiveResidents = await _residentRepository.HasActiveResidentsAsync(unitId);
                return ApiResponse<bool>.CreateSuccess(hasActiveResidents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking active residents for unit ID: {UnitId}", unitId);
                return ApiResponse<bool>.CreateError("Failed to check active residents");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> IsActiveResidentAsync(Guid residentId)
        {
            try
            {
                var isActiveResident = await _residentRepository.IsActiveResidentAsync(residentId);
                return ApiResponse<bool>.CreateSuccess(isActiveResident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if resident is active: {ResidentId}", residentId);
                return ApiResponse<bool>.CreateError("Failed to check if resident is active");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var resident = await _residentRepository.GetByIdWithDetailsAsync(id);
                if (resident == null)
                    return ApiResponse<UnitResident>.CreateError("Resident record not found");

                return ApiResponse<UnitResident>.CreateSuccess(resident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident record details with ID: {Id}", id);
                return ApiResponse<UnitResident>.CreateError("Failed to retrieve resident record details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> SetPrimaryResidentAsync(Guid unitId, Guid residentId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitResident>.CreateError("Unit not found");

                // Get current residents
                var currentResidents = await _residentRepository.GetCurrentResidentsAsync(unitId);
                var targetResident = currentResidents.FirstOrDefault(r => r.ResidentId == residentId);
                
                if (targetResident == null)
                    return ApiResponse<UnitResident>.CreateError("Resident is not currently living in this unit");

                // Remove primary status from current primary resident if exists
                var currentPrimary = currentResidents.FirstOrDefault(r => r.IsPrimary);
                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimary = false;
                    await _residentRepository.UpdateAsync(currentPrimary);
                }

                // Set new primary resident
                targetResident.IsPrimary = true;
                var updatedResident = await _residentRepository.UpdateAsync(targetResident);
                return ApiResponse<UnitResident>.CreateSuccess(updatedResident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting primary resident for unit ID: {UnitId}, resident ID: {ResidentId}", unitId, residentId);
                return ApiResponse<UnitResident>.CreateError("Failed to set primary resident");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResident>> MoveOutResidentAsync(Guid unitId, Guid residentId, DateTime moveOutDate)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitResident>.CreateError("Unit not found");

                var currentResidents = await _residentRepository.GetCurrentResidentsAsync(unitId);
                var resident = currentResidents.FirstOrDefault(r => r.ResidentId == residentId);

                if (resident == null)
                    return ApiResponse<UnitResident>.CreateError("Resident is not currently living in this unit");

                // If this is the primary resident, we need to handle that
                if (resident.IsPrimary)
                {
                    resident.IsPrimary = false;
                    // Note: You might want to add business logic here to automatically assign a new primary resident
                }

                resident.EndDate = moveOutDate;
                var updatedResident = await _residentRepository.UpdateAsync(resident);
                return ApiResponse<UnitResident>.CreateSuccess(updatedResident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving out resident ID: {ResidentId} from unit ID: {UnitId}", residentId, unitId);
                return ApiResponse<UnitResident>.CreateError("Failed to move out resident");
            }
        }
    }
} 