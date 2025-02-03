using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Implementation of IUnitResidentService
    /// </summary>
    public class UnitResidentService : IUnitResidentService
    {
        private readonly IUnitResidentRepository _residentRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitResidentService> _logger;
        private readonly IValidator<CreateUnitResidentRequest> _createValidator;
        private readonly IValidator<UpdateUnitResidentRequest> _updateValidator;

        /// <summary>
        /// Initializes a new instance of the UnitResidentService class.
        /// </summary>
        /// <param name="residentRepository">The unit resident repository.</param>
        /// <param name="unitRepository">The unit repository.</param>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="createValidator">The create validator instance.</param>
        /// <param name="updateValidator">The update validator instance.</param>
        public UnitResidentService(
            IUnitResidentRepository residentRepository,
            IUnitRepository unitRepository,
            IMapper mapper,
            ILogger<UnitResidentService> logger,
            IValidator<CreateUnitResidentRequest> createValidator,
            IValidator<UpdateUnitResidentRequest> updateValidator)
        {
            _residentRepository = residentRepository ?? throw new ArgumentNullException(nameof(residentRepository));
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetAllAsync()
        {
            try
            {
                var residents = await _residentRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<UnitResidentResponse>>(residents);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all resident records");
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateError("Failed to retrieve resident records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateError("Unit not found");

                var residents = await _residentRepository.GetByUnitIdAsync(unitId);
                var response = _mapper.Map<IEnumerable<UnitResidentResponse>>(residents);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident records for unit ID: {UnitId}", unitId);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateError("Failed to retrieve resident records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetByResidentIdAsync(Guid residentId)
        {
            try
            {
                var residents = await _residentRepository.GetByResidentIdAsync(residentId);
                var response = _mapper.Map<IEnumerable<UnitResidentResponse>>(residents);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident records for resident ID: {ResidentId}", residentId);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateError("Failed to retrieve resident records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResidentResponse>>> GetCurrentResidentsAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateError("Unit not found");

                var residents = await _residentRepository.GetCurrentResidentsAsync(unitId);
                var response = _mapper.Map<IEnumerable<UnitResidentResponse>>(residents);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current residents for unit ID: {UnitId}", unitId);
                return ApiResponse<IEnumerable<UnitResidentResponse>>.CreateError("Failed to retrieve current residents");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> GetPrimaryResidentAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitResidentResponse>.CreateError("Unit not found");

                var resident = await _residentRepository.GetPrimaryResidentAsync(unitId);
                if (resident == null)
                    return ApiResponse<UnitResidentResponse>.CreateError("No primary resident found");

                var response = _mapper.Map<UnitResidentResponse>(resident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving primary resident for unit ID: {UnitId}", unitId);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to retrieve primary resident");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var resident = await _residentRepository.GetByIdAsync(id);
                if (resident == null)
                    return ApiResponse<UnitResidentResponse>.CreateError("Resident record not found");

                var response = _mapper.Map<UnitResidentResponse>(resident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident record with ID: {Id}", id);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to retrieve resident record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> CreateAsync(CreateUnitResidentRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UnitResidentResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                if (!await _unitRepository.ExistsAsync(request.UnitId))
                    return ApiResponse<UnitResidentResponse>.CreateError("Unit not found");

                if (request.IsPrimary)
                {
                    var primaryResident = await _residentRepository.GetPrimaryResidentAsync(request.UnitId);
                    if (primaryResident != null)
                        return ApiResponse<UnitResidentResponse>.CreateError("Unit already has a primary resident");
                }

                // 3. Map to entity
                var resident = _mapper.Map<UnitResident>(request);

                // 4. Save to database
                var createdResident = await _residentRepository.AddAsync(resident);

                // 5. Return response
                var response = _mapper.Map<UnitResidentResponse>(createdResident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating resident record for unit ID: {UnitId}", request.UnitId);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to create resident record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> UpdateAsync(UpdateUnitResidentRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UnitResidentResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                var existingResident = await _residentRepository.GetByIdAsync(request.Id);
                if (existingResident == null)
                    return ApiResponse<UnitResidentResponse>.CreateError("Resident record not found");

                if (!await _unitRepository.ExistsAsync(request.UnitId))
                    return ApiResponse<UnitResidentResponse>.CreateError("Unit not found");

                if (request.IsPrimary && !existingResident.IsPrimary)
                {
                    var primaryResident = await _residentRepository.GetPrimaryResidentAsync(request.UnitId);
                    if (primaryResident != null && primaryResident.Id != request.Id)
                        return ApiResponse<UnitResidentResponse>.CreateError("Unit already has a primary resident");
                }

                // 3. Map to entity
                _mapper.Map(request, existingResident);

                // 4. Save to database
                var updatedResident = await _residentRepository.UpdateAsync(existingResident);

                // 5. Return response
                var response = _mapper.Map<UnitResidentResponse>(updatedResident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating resident record with ID: {Id}", request.Id);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to update resident record");
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
        public async Task<ApiResponse<bool>> IsActiveResidentAsync(Guid residentId)
        {
            try
            {
                var isActive = await _residentRepository.IsActiveResidentAsync(residentId);
                return ApiResponse<bool>.CreateSuccess(isActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if resident ID: {ResidentId} is active", residentId);
                return ApiResponse<bool>.CreateError("Failed to check resident status");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var resident = await _residentRepository.GetByIdWithDetailsAsync(id);
                if (resident == null)
                    return ApiResponse<UnitResidentResponse>.CreateError("Resident record not found");

                var response = _mapper.Map<UnitResidentResponse>(resident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident record details with ID: {Id}", id);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to retrieve resident record details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> SetPrimaryResidentAsync(Guid unitId, Guid residentId)
        {
            try
            {
                // 1. Business rules
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitResidentResponse>.CreateError("Unit not found");

                var currentResidents = await _residentRepository.GetCurrentResidentsAsync(unitId);
                var targetResident = currentResidents.FirstOrDefault(r => r.ResidentId == residentId);

                if (targetResident == null)
                    return ApiResponse<UnitResidentResponse>.CreateError("Resident is not currently living in this unit");

                // 2. Remove primary status from current primary resident
                var currentPrimary = currentResidents.FirstOrDefault(r => r.IsPrimary);
                if (currentPrimary != null)
                {
                    currentPrimary.IsPrimary = false;
                    await _residentRepository.UpdateAsync(currentPrimary);
                }

                // 3. Set new primary resident
                targetResident.IsPrimary = true;
                var updatedResident = await _residentRepository.UpdateAsync(targetResident);

                // 4. Return response
                var response = _mapper.Map<UnitResidentResponse>(updatedResident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting primary resident for unit ID: {UnitId}, resident ID: {ResidentId}", unitId, residentId);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to set primary resident");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResidentResponse>> MoveOutResidentAsync(Guid unitId, Guid residentId, DateTime moveOutDate)
        {
            try
            {
                // 1. Business rules
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitResidentResponse>.CreateError("Unit not found");

                var currentResidents = await _residentRepository.GetCurrentResidentsAsync(unitId);
                var resident = currentResidents.FirstOrDefault(r => r.ResidentId == residentId);

                if (resident == null)
                    return ApiResponse<UnitResidentResponse>.CreateError("Resident is not currently living in this unit");

                if (moveOutDate <= resident.StartDate)
                    return ApiResponse<UnitResidentResponse>.CreateError("Move out date must be after move in date");

                // 2. Update resident record
                resident.EndDate = moveOutDate;
                if (resident.IsPrimary)
                {
                    resident.IsPrimary = false;
                    // Note: You might want to add business logic here to automatically assign a new primary resident
                }

                // 3. Save to database
                var updatedResident = await _residentRepository.UpdateAsync(resident);

                // 4. Return response
                var response = _mapper.Map<UnitResidentResponse>(updatedResident);
                return ApiResponse<UnitResidentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while moving out resident ID: {ResidentId} from unit ID: {UnitId}", residentId, unitId);
                return ApiResponse<UnitResidentResponse>.CreateError("Failed to move out resident");
            }
        }
    }
} 