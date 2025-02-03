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
    /// Implementation of IUnitOwnershipService.
    /// Provides business logic operations for managing unit ownership records.
    /// </summary>
    public class UnitOwnershipService : IUnitOwnershipService
    {
        private readonly IUnitOwnershipRepository _ownershipRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly ILogger<UnitOwnershipService> _logger;

        /// <summary>
        /// Initializes a new instance of the UnitOwnershipService class.
        /// </summary>
        /// <param name="ownershipRepository">The unit ownership repository.</param>
        /// <param name="unitRepository">The unit repository.</param>
        /// <param name="logger">The logger instance.</param>
        public UnitOwnershipService(
            IUnitOwnershipRepository ownershipRepository,
            IUnitRepository unitRepository,
            ILogger<UnitOwnershipService> logger)
        {
            _ownershipRepository = ownershipRepository ?? throw new ArgumentNullException(nameof(ownershipRepository));
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitOwnership>>> GetAllAsync()
        {
            try
            {
                var ownerships = await _ownershipRepository.GetAllAsync();
                return ApiResponse<IEnumerable<UnitOwnership>>.CreateSuccess(ownerships);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all unit ownership records");
                return ApiResponse<IEnumerable<UnitOwnership>>.CreateError("Failed to retrieve ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitOwnership>>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<IEnumerable<UnitOwnership>>.CreateError("Unit not found");

                var ownerships = await _ownershipRepository.GetByUnitIdAsync(unitId);
                return ApiResponse<IEnumerable<UnitOwnership>>.CreateSuccess(ownerships);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership records for unit ID: {UnitId}", unitId);
                return ApiResponse<IEnumerable<UnitOwnership>>.CreateError("Failed to retrieve ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitOwnership>>> GetByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                var ownerships = await _ownershipRepository.GetByOwnerIdAsync(ownerId);
                return ApiResponse<IEnumerable<UnitOwnership>>.CreateSuccess(ownerships);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership records for owner ID: {OwnerId}", ownerId);
                return ApiResponse<IEnumerable<UnitOwnership>>.CreateError("Failed to retrieve ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnership>> GetCurrentOwnershipAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitOwnership>.CreateError("Unit not found");

                var ownership = await _ownershipRepository.GetCurrentOwnershipAsync(unitId);
                if (ownership == null)
                    return ApiResponse<UnitOwnership>.CreateError("No current ownership record found");

                return ApiResponse<UnitOwnership>.CreateSuccess(ownership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current ownership for unit ID: {UnitId}", unitId);
                return ApiResponse<UnitOwnership>.CreateError("Failed to retrieve current ownership");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnership>> GetByIdAsync(Guid id)
        {
            try
            {
                var ownership = await _ownershipRepository.GetByIdAsync(id);
                if (ownership == null)
                    return ApiResponse<UnitOwnership>.CreateError("Ownership record not found");

                return ApiResponse<UnitOwnership>.CreateSuccess(ownership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership record with ID: {Id}", id);
                return ApiResponse<UnitOwnership>.CreateError("Failed to retrieve ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnership>> CreateAsync(UnitOwnership ownership)
        {
            try
            {
                if (ownership == null)
                    return ApiResponse<UnitOwnership>.CreateError("Ownership record cannot be null");

                if (!await _unitRepository.ExistsAsync(ownership.UnitId))
                    return ApiResponse<UnitOwnership>.CreateError("Unit not found");

                if (await _ownershipRepository.HasActiveOwnershipAsync(ownership.UnitId))
                    return ApiResponse<UnitOwnership>.CreateError("Unit already has an active owner");

                var createdOwnership = await _ownershipRepository.AddAsync(ownership);
                return ApiResponse<UnitOwnership>.CreateSuccess(createdOwnership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ownership record for unit ID: {UnitId}", ownership?.UnitId);
                return ApiResponse<UnitOwnership>.CreateError("Failed to create ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnership>> UpdateAsync(UnitOwnership ownership)
        {
            try
            {
                if (ownership == null)
                    return ApiResponse<UnitOwnership>.CreateError("Ownership record cannot be null");

                if (!await _ownershipRepository.ExistsAsync(ownership.Id))
                    return ApiResponse<UnitOwnership>.CreateError("Ownership record not found");

                if (!await _unitRepository.ExistsAsync(ownership.UnitId))
                    return ApiResponse<UnitOwnership>.CreateError("Unit not found");

                var updatedOwnership = await _ownershipRepository.UpdateAsync(ownership);
                return ApiResponse<UnitOwnership>.CreateSuccess(updatedOwnership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ownership record with ID: {Id}", ownership?.Id);
                return ApiResponse<UnitOwnership>.CreateError("Failed to update ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _ownershipRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Ownership record not found");

                var result = await _ownershipRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ownership record with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _ownershipRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of ownership record with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check ownership record existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> HasActiveOwnershipAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<bool>.CreateError("Unit not found");

                var hasActiveOwnership = await _ownershipRepository.HasActiveOwnershipAsync(unitId);
                return ApiResponse<bool>.CreateSuccess(hasActiveOwnership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking active ownership for unit ID: {UnitId}", unitId);
                return ApiResponse<bool>.CreateError("Failed to check active ownership");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnership>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var ownership = await _ownershipRepository.GetByIdWithDetailsAsync(id);
                if (ownership == null)
                    return ApiResponse<UnitOwnership>.CreateError("Ownership record not found");

                return ApiResponse<UnitOwnership>.CreateSuccess(ownership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership record details with ID: {Id}", id);
                return ApiResponse<UnitOwnership>.CreateError("Failed to retrieve ownership record details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnership>> TransferOwnershipAsync(Guid unitId, Guid currentOwnerId, Guid newOwnerId, DateTime transferDate)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitOwnership>.CreateError("Unit not found");

                var currentOwnership = await _ownershipRepository.GetCurrentOwnershipAsync(unitId);
                if (currentOwnership == null)
                    return ApiResponse<UnitOwnership>.CreateError("No current ownership record found");

                if (currentOwnership.OwnerId != currentOwnerId)
                    return ApiResponse<UnitOwnership>.CreateError("Current owner ID does not match the unit's current owner");

                // End current ownership
                currentOwnership.EndDate = transferDate;
                await _ownershipRepository.UpdateAsync(currentOwnership);

                // Create new ownership
                var newOwnership = new UnitOwnership
                {
                    UnitId = unitId,
                    OwnerId = newOwnerId,
                    StartDate = transferDate,
                    Status = "Active"
                };

                var createdOwnership = await _ownershipRepository.AddAsync(newOwnership);
                return ApiResponse<UnitOwnership>.CreateSuccess(createdOwnership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while transferring ownership of unit ID: {UnitId} from owner ID: {CurrentOwnerId} to owner ID: {NewOwnerId}",
                    unitId, currentOwnerId, newOwnerId);
                return ApiResponse<UnitOwnership>.CreateError("Failed to transfer ownership");
            }
        }
    }
} 