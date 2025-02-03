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
    /// Implementation of IBlockService
    /// </summary>
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;
        private readonly ISocietyRepository _societyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BlockService> _logger;
        private readonly IValidator<CreateBlockRequest> _createValidator;
        private readonly IValidator<UpdateBlockRequest> _updateValidator;

        /// <summary>
        /// Initializes a new instance of the BlockService class.
        /// </summary>
        /// <param name="blockRepository">The block repository.</param>
        /// <param name="societyRepository">The society repository.</param>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="createValidator">The create validator instance.</param>
        /// <param name="updateValidator">The update validator instance.</param>
        public BlockService(
            IBlockRepository blockRepository,
            ISocietyRepository societyRepository,
            IMapper mapper,
            ILogger<BlockService> logger,
            IValidator<CreateBlockRequest> createValidator,
            IValidator<UpdateBlockRequest> updateValidator)
        {
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
            _societyRepository = societyRepository ?? throw new ArgumentNullException(nameof(societyRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<BlockResponse>>> GetAllAsync()
        {
            try
            {
                var blocks = await _blockRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<BlockResponse>>(blocks);
                return ApiResponse<IEnumerable<BlockResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all blocks");
                return ApiResponse<IEnumerable<BlockResponse>>.CreateError("Failed to retrieve blocks");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<BlockResponse>>> GetBySocietyIdAsync(Guid societyId)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(societyId))
                    return ApiResponse<IEnumerable<BlockResponse>>.CreateError("Society not found");

                var blocks = await _blockRepository.GetBySocietyIdAsync(societyId);
                var response = _mapper.Map<IEnumerable<BlockResponse>>(blocks);
                return ApiResponse<IEnumerable<BlockResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving blocks for society ID: {SocietyId}", societyId);
                return ApiResponse<IEnumerable<BlockResponse>>.CreateError("Failed to retrieve blocks");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<BlockResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var block = await _blockRepository.GetByIdAsync(id);
                if (block == null)
                    return ApiResponse<BlockResponse>.CreateError("Block not found");

                var response = _mapper.Map<BlockResponse>(block);
                return ApiResponse<BlockResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving block with ID: {Id}", id);
                return ApiResponse<BlockResponse>.CreateError("Failed to retrieve block");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<BlockResponse>> CreateAsync(CreateBlockRequest request)
        {
            try
            {
                // Validate request
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<BlockResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // Check if society exists
                if (!await _societyRepository.ExistsAsync(request.SocietyId))
                    return ApiResponse<BlockResponse>.CreateError("Society not found");

                // Check for unique name within society
                if (!await _blockRepository.IsNameUniqueInSocietyAsync(request.SocietyId, request.Name))
                    return ApiResponse<BlockResponse>.CreateError("Block name must be unique within the society");

                // Map and create
                var block = _mapper.Map<Block>(request);
                var createdBlock = await _blockRepository.AddAsync(block);
                var response = _mapper.Map<BlockResponse>(createdBlock);
                return ApiResponse<BlockResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating block: {Name}", request.Name);
                return ApiResponse<BlockResponse>.CreateError("Failed to create block");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<BlockResponse>> UpdateAsync(UpdateBlockRequest request)
        {
            try
            {
                // Validate request
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<BlockResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // Check if block exists
                var existingBlock = await _blockRepository.GetByIdAsync(request.Id);
                if (existingBlock == null)
                    return ApiResponse<BlockResponse>.CreateError("Block not found");

                // Check for unique name within society
                if (!await _blockRepository.IsNameUniqueInSocietyAsync(existingBlock.SocietyId, request.Name, request.Id))
                    return ApiResponse<BlockResponse>.CreateError("Block name must be unique within the society");

                // Map and update
                _mapper.Map(request, existingBlock);
                var updatedBlock = await _blockRepository.UpdateAsync(existingBlock);
                var response = _mapper.Map<BlockResponse>(updatedBlock);
                return ApiResponse<BlockResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating block with ID: {Id}", request.Id);
                return ApiResponse<BlockResponse>.CreateError("Failed to update block");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _blockRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Block not found");

                var result = await _blockRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting block with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete block");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _blockRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of block with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check block existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> IsNameUniqueInSocietyAsync(Guid societyId, string name, Guid? excludeId = null)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(societyId))
                    return ApiResponse<bool>.CreateError("Society not found");

                var isUnique = await _blockRepository.IsNameUniqueInSocietyAsync(societyId, name, excludeId);
                return ApiResponse<bool>.CreateSuccess(isUnique);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking uniqueness of block name {Name} in society {SocietyId}", name, societyId);
                return ApiResponse<bool>.CreateError("Failed to check block name uniqueness");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<BlockResponse>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var block = await _blockRepository.GetByIdWithDetailsAsync(id);
                if (block == null)
                    return ApiResponse<BlockResponse>.CreateError("Block not found");

                var response = _mapper.Map<BlockResponse>(block);
                return ApiResponse<BlockResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving block details with ID: {Id}", id);
                return ApiResponse<BlockResponse>.CreateError("Failed to retrieve block details");
            }
        }
    }
} 