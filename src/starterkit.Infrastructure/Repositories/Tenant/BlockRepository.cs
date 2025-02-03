using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Application.Persistence;

namespace starterkit.Infrastructure.Repositories.Tenant
{
    /// <summary>
    /// Implementation of IBlockRepository.
    /// Handles data access operations for Block entities using Entity Framework Core.
    /// </summary>
    public class BlockRepository : IBlockRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<BlockRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the BlockRepository class.
        /// </summary>
        /// <param name="context">The tenant database context.</param>
        /// <param name="logger">The logger instance for this repository.</param>
        public BlockRepository(ITenantDbContext context, ILogger<BlockRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Block>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all blocks");
                return await _context.Blocks
                    .Where(b => b.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all blocks");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Block>> GetBySocietyIdAsync(Guid societyId)
        {
            try
            {
                _logger.LogInformation("Retrieving blocks for society ID: {SocietyId}", societyId);
                return await _context.Blocks
                    .Where(b => b.SocietyId == societyId && b.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving blocks for society ID: {SocietyId}", societyId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Block> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving block with ID: {Id}", id);
                return await _context.Blocks
                    .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving block with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Block> AddAsync(Block block)
        {
            try
            {
                _logger.LogInformation("Adding new block: {Name} for society: {SocietyId}", block.Name, block.SocietyId);
                await _context.Blocks.AddAsync(block);
                await _context.SaveChangesAsync();
                return block;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding block: {Name}", block.Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Block> UpdateAsync(Block block)
        {
            try
            {
                _logger.LogInformation("Updating block with ID: {Id}", block.Id);
                _context.Blocks.Update(block);
                await _context.SaveChangesAsync();
                return block;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating block with ID: {Id}", block.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Checking if block exists with ID: {Id}", id);
                return await _context.Blocks
                    .AnyAsync(b => b.Id == id && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of block with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsNameUniqueInSocietyAsync(Guid societyId, string name, Guid? excludeId = null)
        {
            try
            {
                _logger.LogInformation("Checking if block name {Name} is unique in society {SocietyId}", name, societyId);
                var query = _context.Blocks
                    .Where(b => b.SocietyId == societyId && 
                               b.Name == name && 
                               b.IsActive);

                if (excludeId.HasValue)
                {
                    query = query.Where(b => b.Id != excludeId.Value);
                }

                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking uniqueness of block name {Name} in society {SocietyId}", name, societyId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Soft deleting block with ID: {Id}", id);
                var block = await _context.Blocks.FindAsync(id);
                if (block == null || !block.IsActive)
                    return false;

                block.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting block with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Block> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving block with details for ID: {Id}", id);
                return await _context.Blocks
                    .Include(b => b.Society)
                    .Include(b => b.Floors)
                        .ThenInclude(f => f.Units)
                    .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving block with details for ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<Block>, int)> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Retrieving paged blocks. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                
                var query = _context.Blocks.Where(b => b.IsActive);
                var totalCount = await query.CountAsync();
                
                var blocks = await query
                    .OrderBy(b => b.Name)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (blocks, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged blocks");
                throw;
            }
        }
    }
} 