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
    /// Implementation of IFloorRepository.
    /// Handles data access operations for Floor entities using Entity Framework Core.
    /// </summary>
    public class FloorRepository : IFloorRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<FloorRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the FloorRepository class.
        /// </summary>
        /// <param name="context">The tenant database context.</param>
        /// <param name="logger">The logger instance for this repository.</param>
        public FloorRepository(ITenantDbContext context, ILogger<FloorRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Floor>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all floors");
                return await _context.Floors
                    .Where(f => f.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all floors");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Floor>> GetByBlockIdAsync(Guid blockId)
        {
            try
            {
                _logger.LogInformation("Retrieving floors for block ID: {BlockId}", blockId);
                return await _context.Floors
                    .Where(f => f.BlockId == blockId && f.IsActive)
                    .OrderBy(f => f.FloorNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floors for block ID: {BlockId}", blockId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Floor> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving floor with ID: {Id}", id);
                return await _context.Floors
                    .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Floor> AddAsync(Floor floor)
        {
            try
            {
                _logger.LogInformation("Adding new floor: {Name} for block: {BlockId}", floor.Name, floor.BlockId);
                await _context.Floors.AddAsync(floor);
                await _context.SaveChangesAsync();
                return floor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding floor: {Name}", floor.Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Floor> UpdateAsync(Floor floor)
        {
            try
            {
                _logger.LogInformation("Updating floor with ID: {Id}", floor.Id);
                _context.Floors.Update(floor);
                await _context.SaveChangesAsync();
                return floor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating floor with ID: {Id}", floor.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Checking if floor exists with ID: {Id}", id);
                return await _context.Floors
                    .AnyAsync(f => f.Id == id && f.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of floor with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsFloorNumberUniqueInBlockAsync(Guid blockId, int floorNumber, Guid? excludeId = null)
        {
            try
            {
                _logger.LogInformation("Checking if floor number {FloorNumber} is unique in block {BlockId}", floorNumber, blockId);
                var query = _context.Floors
                    .Where(f => f.BlockId == blockId && 
                               f.FloorNumber == floorNumber && 
                               f.IsActive);

                if (excludeId.HasValue)
                {
                    query = query.Where(f => f.Id != excludeId.Value);
                }

                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking uniqueness of floor number {FloorNumber} in block {BlockId}", floorNumber, blockId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Soft deleting floor with ID: {Id}", id);
                var floor = await _context.Floors.FindAsync(id);
                if (floor == null || !floor.IsActive)
                    return false;

                floor.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting floor with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Floor> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving floor with details for ID: {Id}", id);
                return await _context.Floors
                    .Include(f => f.Block)
                        .ThenInclude(b => b.Society)
                    .Include(f => f.Units)
                    .FirstOrDefaultAsync(f => f.Id == id && f.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor with details for ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<Floor>, int)> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Retrieving paged floors. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                
                var query = _context.Floors.Where(f => f.IsActive);
                var totalCount = await query.CountAsync();
                
                var floors = await query
                    .OrderBy(f => f.Block.Name)
                    .ThenBy(f => f.FloorNumber)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (floors, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged floors");
                throw;
            }
        }
    }
} 