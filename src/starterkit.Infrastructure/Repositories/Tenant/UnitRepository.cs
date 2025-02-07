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
    /// Implementation of IUnitRepository.
    /// Handles data access operations for Unit entities using Entity Framework Core.
    /// </summary>
    public class UnitRepository : IUnitRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<UnitRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the UnitRepository class.
        /// </summary>
        /// <param name="context">The tenant database context.</param>
        /// <param name="logger">The logger instance for this repository.</param>
        public UnitRepository(ITenantDbContext context, ILogger<UnitRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Unit>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all units");
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .Where(u => u.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all units");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Unit>> GetByFloorIdAsync(Guid floorId)
        {
            try
            {
                _logger.LogInformation("Retrieving units for floor ID: {FloorId}", floorId);
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .Where(u => u.FloorId == floorId && u.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for floor ID: {FloorId}", floorId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Unit>> GetByBlockIdAsync(Guid blockId)
        {
            try
            {
                _logger.LogInformation("Retrieving units for block ID: {BlockId}", blockId);
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .Where(u => u.Floor.BlockId == blockId && u.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for block ID: {BlockId}", blockId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Unit>> GetBySocietyIdAsync(Guid societyId)
        {
            try
            {
                _logger.LogInformation("Retrieving units for society ID: {SocietyId}", societyId);
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .Where(u => u.Floor.Block.SocietyId == societyId && u.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for society ID: {SocietyId}", societyId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Unit> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving unit with ID: {Id}", id);
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Unit> GetByUnitNumberAsync(Guid societyId, string unitNumber)
        {
            try
            {
                _logger.LogInformation("Retrieving unit with number: {UnitNumber} in society: {SocietyId}", unitNumber, societyId);
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .FirstOrDefaultAsync(u => u.Floor.Block.SocietyId == societyId && 
                                            u.UnitNumber == unitNumber && 
                                            u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit with number: {UnitNumber}", unitNumber);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Unit> AddAsync(Unit unit)
        {
            try
            {
                _logger.LogInformation("Adding new unit: {UnitNumber} for floor: {FloorId}", unit.UnitNumber, unit.FloorId);
                await _context.Units.AddAsync(unit);
                await _context.SaveChangesAsync();
                return unit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding unit: {UnitNumber}", unit.UnitNumber);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Unit> UpdateAsync(Unit unit)
        {
            try
            {
                _logger.LogInformation("Updating unit with ID: {Id}", unit.Id);
                _context.Units.Update(unit);
                await _context.SaveChangesAsync();
                return unit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating unit with ID: {Id}", unit.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Checking if unit exists with ID: {Id}", id);
                return await _context.Units
                    .AnyAsync(u => u.Id == id && u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of unit with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsUnitNumberUniqueInSocietyAsync(Guid societyId, string unitNumber, Guid? excludeId = null)
        {
            try
            {
                _logger.LogInformation("Checking if unit number {UnitNumber} is unique in society {SocietyId}", unitNumber, societyId);
                var query = _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .Where(u => u.Floor.Block.SocietyId == societyId && 
                               u.UnitNumber == unitNumber && 
                               u.IsActive);

                if (excludeId.HasValue)
                {
                    query = query.Where(u => u.Id != excludeId.Value);
                }

                return !await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking uniqueness of unit number {UnitNumber} in society {SocietyId}", unitNumber, societyId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Soft deleting unit with ID: {Id}", id);
                var unit = await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .FirstOrDefaultAsync(u => u.Id == id);
                    
                if (unit == null || !unit.IsActive)
                    return false;

                unit.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting unit with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Unit> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving unit with details for ID: {Id}", id);
                return await _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                            .ThenInclude(b => b.Society)
                    .Include(u => u.Ownerships)
                    .Include(u => u.Residents)
                    .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit with details for ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<Unit>, int)> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Retrieving paged units. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                
                var query = _context.Units
                    .Include(u => u.Floor)
                        .ThenInclude(f => f.Block)
                    .Where(u => u.IsActive);
                
                var totalCount = await query.CountAsync();
                
                var units = await query
                    .OrderBy(u => u.Floor.Block.Name)
                    .ThenBy(u => u.Floor.FloorNumber)
                    .ThenBy(u => u.UnitNumber)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (units, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged units");
                throw;
            }
        }
    }
} 