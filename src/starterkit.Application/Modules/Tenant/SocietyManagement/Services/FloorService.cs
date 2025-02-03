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
    /// Implementation of IFloorService.
    /// Provides business logic operations for managing floors.
    /// </summary>
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly ILogger<FloorService> _logger;

        /// <summary>
        /// Initializes a new instance of the FloorService class.
        /// </summary>
        /// <param name="floorRepository">The floor repository.</param>
        /// <param name="blockRepository">The block repository.</param>
        /// <param name="logger">The logger instance.</param>
        public FloorService(
            IFloorRepository floorRepository,
            IBlockRepository blockRepository,
            ILogger<FloorService> logger)
        {
            _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Floor>>> GetAllAsync()
        {
            try
            {
                var floors = await _floorRepository.GetAllAsync();
                return ApiResponse<IEnumerable<Floor>>.CreateSuccess(floors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all floors");
                return ApiResponse<IEnumerable<Floor>>.CreateError("Failed to retrieve floors");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Floor>>> GetByBlockIdAsync(Guid blockId)
        {
            try
            {
                if (!await _blockRepository.ExistsAsync(blockId))
                    return ApiResponse<IEnumerable<Floor>>.CreateError("Block not found");

                var floors = await _floorRepository.GetByBlockIdAsync(blockId);
                return ApiResponse<IEnumerable<Floor>>.CreateSuccess(floors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floors for block ID: {BlockId}", blockId);
                return ApiResponse<IEnumerable<Floor>>.CreateError("Failed to retrieve floors");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Floor>> GetByIdAsync(Guid id)
        {
            try
            {
                var floor = await _floorRepository.GetByIdAsync(id);
                if (floor == null)
                    return ApiResponse<Floor>.CreateError("Floor not found");

                return ApiResponse<Floor>.CreateSuccess(floor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor with ID: {Id}", id);
                return ApiResponse<Floor>.CreateError("Failed to retrieve floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Floor>> CreateAsync(Floor floor)
        {
            try
            {
                if (floor == null)
                    return ApiResponse<Floor>.CreateError("Floor cannot be null");

                if (!await _blockRepository.ExistsAsync(floor.BlockId))
                    return ApiResponse<Floor>.CreateError("Block not found");

                if (!await _floorRepository.IsFloorNumberUniqueInBlockAsync(floor.BlockId, floor.FloorNumber))
                    return ApiResponse<Floor>.CreateError("Floor number must be unique within the block");

                var createdFloor = await _floorRepository.AddAsync(floor);
                return ApiResponse<Floor>.CreateSuccess(createdFloor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating floor: {Name}", floor?.Name);
                return ApiResponse<Floor>.CreateError("Failed to create floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Floor>> UpdateAsync(Floor floor)
        {
            try
            {
                if (floor == null)
                    return ApiResponse<Floor>.CreateError("Floor cannot be null");

                if (!await _floorRepository.ExistsAsync(floor.Id))
                    return ApiResponse<Floor>.CreateError("Floor not found");

                if (!await _blockRepository.ExistsAsync(floor.BlockId))
                    return ApiResponse<Floor>.CreateError("Block not found");

                if (!await _floorRepository.IsFloorNumberUniqueInBlockAsync(floor.BlockId, floor.FloorNumber, floor.Id))
                    return ApiResponse<Floor>.CreateError("Floor number must be unique within the block");

                var updatedFloor = await _floorRepository.UpdateAsync(floor);
                return ApiResponse<Floor>.CreateSuccess(updatedFloor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating floor with ID: {Id}", floor?.Id);
                return ApiResponse<Floor>.CreateError("Failed to update floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _floorRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Floor not found");

                var result = await _floorRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting floor with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _floorRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of floor with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check floor existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> IsFloorNumberUniqueInBlockAsync(Guid blockId, int floorNumber, Guid? excludeId = null)
        {
            try
            {
                if (!await _blockRepository.ExistsAsync(blockId))
                    return ApiResponse<bool>.CreateError("Block not found");

                var isUnique = await _floorRepository.IsFloorNumberUniqueInBlockAsync(blockId, floorNumber, excludeId);
                return ApiResponse<bool>.CreateSuccess(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking uniqueness of floor number {FloorNumber} in block {BlockId}", floorNumber, blockId);
                return ApiResponse<bool>.CreateError("Failed to check floor number uniqueness");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Floor>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var floor = await _floorRepository.GetByIdWithDetailsAsync(id);
                if (floor == null)
                    return ApiResponse<Floor>.CreateError("Floor not found");

                return ApiResponse<Floor>.CreateSuccess(floor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor details with ID: {Id}", id);
                return ApiResponse<Floor>.CreateError("Failed to retrieve floor details");
            }
        }
    }
} 