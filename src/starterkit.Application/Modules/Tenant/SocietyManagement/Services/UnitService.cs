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
    /// Implementation of IUnitService.
    /// Provides business logic operations for managing units.
    /// </summary>
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly ISocietyRepository _societyRepository;
        private readonly ILogger<UnitService> _logger;

        /// <summary>
        /// Initializes a new instance of the UnitService class.
        /// </summary>
        /// <param name="unitRepository">The unit repository.</param>
        /// <param name="floorRepository">The floor repository.</param>
        /// <param name="blockRepository">The block repository.</param>
        /// <param name="societyRepository">The society repository.</param>
        /// <param name="logger">The logger instance.</param>
        public UnitService(
            IUnitRepository unitRepository,
            IFloorRepository floorRepository,
            IBlockRepository blockRepository,
            ISocietyRepository societyRepository,
            ILogger<UnitService> logger)
        {
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
            _societyRepository = societyRepository ?? throw new ArgumentNullException(nameof(societyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Unit>>> GetAllAsync()
        {
            try
            {
                var units = await _unitRepository.GetAllAsync();
                return ApiResponse<IEnumerable<Unit>>.CreateSuccess(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all units");
                return ApiResponse<IEnumerable<Unit>>.CreateError("Failed to retrieve units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Unit>>> GetByFloorIdAsync(Guid floorId)
        {
            try
            {
                if (!await _floorRepository.ExistsAsync(floorId))
                    return ApiResponse<IEnumerable<Unit>>.CreateError("Floor not found");

                var units = await _unitRepository.GetByFloorIdAsync(floorId);
                return ApiResponse<IEnumerable<Unit>>.CreateSuccess(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for floor ID: {FloorId}", floorId);
                return ApiResponse<IEnumerable<Unit>>.CreateError("Failed to retrieve units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Unit>>> GetByBlockIdAsync(Guid blockId)
        {
            try
            {
                if (!await _blockRepository.ExistsAsync(blockId))
                    return ApiResponse<IEnumerable<Unit>>.CreateError("Block not found");

                var units = await _unitRepository.GetByBlockIdAsync(blockId);
                return ApiResponse<IEnumerable<Unit>>.CreateSuccess(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for block ID: {BlockId}", blockId);
                return ApiResponse<IEnumerable<Unit>>.CreateError("Failed to retrieve units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Unit>>> GetBySocietyIdAsync(Guid societyId)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(societyId))
                    return ApiResponse<IEnumerable<Unit>>.CreateError("Society not found");

                var units = await _unitRepository.GetBySocietyIdAsync(societyId);
                return ApiResponse<IEnumerable<Unit>>.CreateSuccess(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for society ID: {SocietyId}", societyId);
                return ApiResponse<IEnumerable<Unit>>.CreateError("Failed to retrieve units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Unit>> GetByIdAsync(Guid id)
        {
            try
            {
                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null)
                    return ApiResponse<Unit>.CreateError("Unit not found");

                return ApiResponse<Unit>.CreateSuccess(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit with ID: {Id}", id);
                return ApiResponse<Unit>.CreateError("Failed to retrieve unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Unit>> GetByUnitNumberAsync(Guid societyId, string unitNumber)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(societyId))
                    return ApiResponse<Unit>.CreateError("Society not found");

                var unit = await _unitRepository.GetByUnitNumberAsync(societyId, unitNumber);
                if (unit == null)
                    return ApiResponse<Unit>.CreateError("Unit not found");

                return ApiResponse<Unit>.CreateSuccess(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit with number: {UnitNumber}", unitNumber);
                return ApiResponse<Unit>.CreateError("Failed to retrieve unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Unit>> CreateAsync(Unit unit)
        {
            try
            {
                if (unit == null)
                    return ApiResponse<Unit>.CreateError("Unit cannot be null");

                if (!await _floorRepository.ExistsAsync(unit.FloorId))
                    return ApiResponse<Unit>.CreateError("Floor not found");

                var floor = await _floorRepository.GetByIdWithDetailsAsync(unit.FloorId);
                if (!await _unitRepository.IsUnitNumberUniqueInSocietyAsync(floor.Block.SocietyId, unit.UnitNumber))
                    return ApiResponse<Unit>.CreateError("Unit number must be unique within the society");

                var createdUnit = await _unitRepository.AddAsync(unit);
                return ApiResponse<Unit>.CreateSuccess(createdUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating unit: {UnitNumber}", unit?.UnitNumber);
                return ApiResponse<Unit>.CreateError("Failed to create unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Unit>> UpdateAsync(Unit unit)
        {
            try
            {
                if (unit == null)
                    return ApiResponse<Unit>.CreateError("Unit cannot be null");

                if (!await _unitRepository.ExistsAsync(unit.Id))
                    return ApiResponse<Unit>.CreateError("Unit not found");

                if (!await _floorRepository.ExistsAsync(unit.FloorId))
                    return ApiResponse<Unit>.CreateError("Floor not found");

                var floor = await _floorRepository.GetByIdWithDetailsAsync(unit.FloorId);
                if (!await _unitRepository.IsUnitNumberUniqueInSocietyAsync(floor.Block.SocietyId, unit.UnitNumber, unit.Id))
                    return ApiResponse<Unit>.CreateError("Unit number must be unique within the society");

                var updatedUnit = await _unitRepository.UpdateAsync(unit);
                return ApiResponse<Unit>.CreateSuccess(updatedUnit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating unit with ID: {Id}", unit?.Id);
                return ApiResponse<Unit>.CreateError("Failed to update unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Unit not found");

                var result = await _unitRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting unit with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _unitRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of unit with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check unit existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> IsUnitNumberUniqueInSocietyAsync(Guid societyId, string unitNumber, Guid? excludeId = null)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(societyId))
                    return ApiResponse<bool>.CreateError("Society not found");

                var isUnique = await _unitRepository.IsUnitNumberUniqueInSocietyAsync(societyId, unitNumber, excludeId);
                return ApiResponse<bool>.CreateSuccess(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking uniqueness of unit number {UnitNumber} in society {SocietyId}", unitNumber, societyId);
                return ApiResponse<bool>.CreateError("Failed to check unit number uniqueness");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Unit>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var unit = await _unitRepository.GetByIdWithDetailsAsync(id);
                if (unit == null)
                    return ApiResponse<Unit>.CreateError("Unit not found");

                return ApiResponse<Unit>.CreateSuccess(unit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit details with ID: {Id}", id);
                return ApiResponse<Unit>.CreateError("Failed to retrieve unit details");
            }
        }
    }
} 