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
    /// Implementation of IBlockService.
    /// Provides business logic operations for managing blocks.
    /// </summary>
    public class BlockService : IBlockService
    {
        private readonly IBlockRepository _blockRepository;
        private readonly ISocietyRepository _societyRepository;
        private readonly ILogger<BlockService> _logger;

        /// <summary>
        /// Initializes a new instance of the BlockService class.
        /// </summary>
        /// <param name="blockRepository">The block repository.</param>
        /// <param name="societyRepository">The society repository.</param>
        /// <param name="logger">The logger instance.</param>
        public BlockService(
            IBlockRepository blockRepository,
            ISocietyRepository societyRepository,
            ILogger<BlockService> logger)
        {
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
            _societyRepository = societyRepository ?? throw new ArgumentNullException(nameof(societyRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Block>>> GetAllAsync()
        {
            try
            {
                var blocks = await _blockRepository.GetAllAsync();
                return ApiResponse<IEnumerable<Block>>.CreateSuccess(blocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all blocks");
                return ApiResponse<IEnumerable<Block>>.CreateError("Failed to retrieve blocks");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<Block>>> GetBySocietyIdAsync(Guid societyId)
        {
            try
            {
                if (!await _societyRepository.ExistsAsync(societyId))
                    return ApiResponse<IEnumerable<Block>>.CreateError("Society not found");

                var blocks = await _blockRepository.GetBySocietyIdAsync(societyId);
                return ApiResponse<IEnumerable<Block>>.CreateSuccess(blocks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving blocks for society ID: {SocietyId}", societyId);
                return ApiResponse<IEnumerable<Block>>.CreateError("Failed to retrieve blocks");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Block>> GetByIdAsync(Guid id)
        {
            try
            {
                var block = await _blockRepository.GetByIdAsync(id);
                if (block == null)
                    return ApiResponse<Block>.CreateError("Block not found");

                return ApiResponse<Block>.CreateSuccess(block);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving block with ID: {Id}", id);
                return ApiResponse<Block>.CreateError("Failed to retrieve block");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Block>> CreateAsync(Block block)
        {
            try
            {
                if (block == null)
                    return ApiResponse<Block>.CreateError("Block cannot be null");

                if (!await _societyRepository.ExistsAsync(block.SocietyId))
                    return ApiResponse<Block>.CreateError("Society not found");

                if (!await _blockRepository.IsNameUniqueInSocietyAsync(block.SocietyId, block.Name))
                    return ApiResponse<Block>.CreateError("Block name must be unique within the society");

                var createdBlock = await _blockRepository.AddAsync(block);
                return ApiResponse<Block>.CreateSuccess(createdBlock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating block: {Name}", block?.Name);
                return ApiResponse<Block>.CreateError("Failed to create block");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<Block>> UpdateAsync(Block block)
        {
            try
            {
                if (block == null)
                    return ApiResponse<Block>.CreateError("Block cannot be null");

                if (!await _blockRepository.ExistsAsync(block.Id))
                    return ApiResponse<Block>.CreateError("Block not found");

                if (!await _societyRepository.ExistsAsync(block.SocietyId))
                    return ApiResponse<Block>.CreateError("Society not found");

                if (!await _blockRepository.IsNameUniqueInSocietyAsync(block.SocietyId, block.Name, block.Id))
                    return ApiResponse<Block>.CreateError("Block name must be unique within the society");

                var updatedBlock = await _blockRepository.UpdateAsync(block);
                return ApiResponse<Block>.CreateSuccess(updatedBlock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating block with ID: {Id}", block?.Id);
                return ApiResponse<Block>.CreateError("Failed to update block");
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
        public async Task<ApiResponse<Block>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var block = await _blockRepository.GetByIdWithDetailsAsync(id);
                if (block == null)
                    return ApiResponse<Block>.CreateError("Block not found");

                return ApiResponse<Block>.CreateSuccess(block);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving block details with ID: {Id}", id);
                return ApiResponse<Block>.CreateError("Failed to retrieve block details");
            }
        }
    }
} 