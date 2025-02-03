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
    /// Implementation of IFloorService
    /// </summary>
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FloorService> _logger;
        private readonly IValidator<CreateFloorRequest> _createValidator;
        private readonly IValidator<UpdateFloorRequest> _updateValidator;

        /// <summary>
        /// Initializes a new instance of the FloorService class.
        /// </summary>
        /// <param name="floorRepository">The floor repository.</param>
        /// <param name="blockRepository">The block repository.</param>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="createValidator">The create validator instance.</param>
        /// <param name="updateValidator">The update validator instance.</param>
        public FloorService(
            IFloorRepository floorRepository,
            IBlockRepository blockRepository,
            IMapper mapper,
            ILogger<FloorService> logger,
            IValidator<CreateFloorRequest> createValidator,
            IValidator<UpdateFloorRequest> updateValidator)
        {
            _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<FloorResponse>>> GetAllAsync()
        {
            try
            {
                var floors = await _floorRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<FloorResponse>>(floors);
                return ApiResponse<IEnumerable<FloorResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all floors");
                return ApiResponse<IEnumerable<FloorResponse>>.CreateError("Failed to retrieve floors");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<FloorResponse>>> GetByBlockIdAsync(Guid blockId)
        {
            try
            {
                if (!await _blockRepository.ExistsAsync(blockId))
                    return ApiResponse<IEnumerable<FloorResponse>>.CreateError("Block not found");

                var floors = await _floorRepository.GetByBlockIdAsync(blockId);
                var response = _mapper.Map<IEnumerable<FloorResponse>>(floors);
                return ApiResponse<IEnumerable<FloorResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floors for block ID: {BlockId}", blockId);
                return ApiResponse<IEnumerable<FloorResponse>>.CreateError("Failed to retrieve floors");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<FloorResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var floor = await _floorRepository.GetByIdAsync(id);
                if (floor == null)
                    return ApiResponse<FloorResponse>.CreateError("Floor not found");

                var response = _mapper.Map<FloorResponse>(floor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor with ID: {Id}", id);
                return ApiResponse<FloorResponse>.CreateError("Failed to retrieve floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<FloorResponse>> CreateAsync(CreateFloorRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<FloorResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                if (!await _blockRepository.ExistsAsync(request.BlockId))
                    return ApiResponse<FloorResponse>.CreateError("Block not found");

                if (!await _floorRepository.IsFloorNumberUniqueInBlockAsync(request.BlockId, request.FloorNumber))
                    return ApiResponse<FloorResponse>.CreateError("Floor number must be unique within the block");

                // 3. Map to entity
                var floor = _mapper.Map<Floor>(request);

                // 4. Save to database
                var createdFloor = await _floorRepository.AddAsync(floor);

                // 5. Return response
                var response = _mapper.Map<FloorResponse>(createdFloor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating floor: {Name}", request.Name);
                return ApiResponse<FloorResponse>.CreateError("Failed to create floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<FloorResponse>> UpdateAsync(UpdateFloorRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<FloorResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                var existingFloor = await _floorRepository.GetByIdAsync(request.Id);
                if (existingFloor == null)
                    return ApiResponse<FloorResponse>.CreateError("Floor not found");

                if (!await _blockRepository.ExistsAsync(request.BlockId))
                    return ApiResponse<FloorResponse>.CreateError("Block not found");

                if (!await _floorRepository.IsFloorNumberUniqueInBlockAsync(request.BlockId, request.FloorNumber, request.Id))
                    return ApiResponse<FloorResponse>.CreateError("Floor number must be unique within the block");

                // 3. Map to entity
                _mapper.Map(request, existingFloor);

                // 4. Save to database
                var updatedFloor = await _floorRepository.UpdateAsync(existingFloor);

                // 5. Return response
                var response = _mapper.Map<FloorResponse>(updatedFloor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating floor with ID: {Id}", request.Id);
                return ApiResponse<FloorResponse>.CreateError("Failed to update floor");
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
        public async Task<ApiResponse<FloorResponse>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var floor = await _floorRepository.GetByIdWithDetailsAsync(id);
                if (floor == null)
                    return ApiResponse<FloorResponse>.CreateError("Floor not found");

                var response = _mapper.Map<FloorResponse>(floor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor details with ID: {Id}", id);
                return ApiResponse<FloorResponse>.CreateError("Failed to retrieve floor details");
            }
        }
    }
} 