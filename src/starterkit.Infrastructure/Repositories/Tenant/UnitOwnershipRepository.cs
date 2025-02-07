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
    /// Implementation of IUnitOwnershipRepository.
    /// Handles data access operations for UnitOwnership entities using Entity Framework Core.
    /// </summary>
    public class UnitOwnershipRepository : IUnitOwnershipRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<UnitOwnershipRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the UnitOwnershipRepository class.
        /// </summary>
        /// <param name="context">The tenant database context.</param>
        /// <param name="logger">The logger instance.</param>
        public UnitOwnershipRepository(ITenantDbContext context, ILogger<UnitOwnershipRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitOwnership>> GetAllAsync()
        {
            try
            {
                return await _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                    .Include(uo => uo.Owner)
                        .ThenInclude(o => o.Profile)
                    .Where(uo => uo.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all unit ownership records");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitOwnership>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                return await _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                    .Include(uo => uo.Owner)
                        .ThenInclude(o => o.Profile)
                    .Where(uo => uo.UnitId == unitId && uo.IsActive)
                    .OrderByDescending(uo => uo.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership records for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitOwnership>> GetByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                return await _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                    .Include(uo => uo.Owner)
                        .ThenInclude(o => o.Profile)
                    .Where(uo => uo.OwnerId == ownerId && uo.IsActive)
                    .OrderByDescending(uo => uo.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership records for owner ID: {OwnerId}", ownerId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitOwnership> GetCurrentOwnershipAsync(Guid unitId)
        {
            try
            {
                return await _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                    .Include(uo => uo.Owner)
                        .ThenInclude(o => o.Profile)
                    .Where(uo => uo.UnitId == unitId && 
                                uo.IsActive && 
                                uo.EndDate == null)
                    .OrderByDescending(uo => uo.StartDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current ownership for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitOwnership> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                    .Include(uo => uo.Owner)
                        .ThenInclude(o => o.Profile)
                    .FirstOrDefaultAsync(uo => uo.Id == id && uo.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership record with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitOwnership> AddAsync(UnitOwnership ownership)
        {
            try
            {
                await _context.UnitOwnerships.AddAsync(ownership);
                await _context.SaveChangesAsync();
                return ownership;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding ownership record for unit ID: {UnitId}", ownership.UnitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitOwnership> UpdateAsync(UnitOwnership ownership)
        {
            try
            {
                _context.UnitOwnerships.Update(ownership);
                await _context.SaveChangesAsync();
                return ownership;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ownership record with ID: {Id}", ownership.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                return await _context.UnitOwnerships
                    .AnyAsync(uo => uo.Id == id && uo.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of ownership record with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> HasActiveOwnershipAsync(Guid unitId)
        {
            try
            {
                return await _context.UnitOwnerships
                    .AnyAsync(uo => uo.UnitId == unitId && 
                                  uo.IsActive && 
                                  uo.EndDate == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking active ownership for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var ownership = await _context.UnitOwnerships.FindAsync(id);
                if (ownership == null || !ownership.IsActive)
                    return false;

                ownership.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ownership record with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitOwnership> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                return await _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                        .ThenInclude(u => u.Floor)
                            .ThenInclude(f => f.Block)
                                .ThenInclude(b => b.Society)
                    .Include(uo => uo.Owner)
                    .FirstOrDefaultAsync(uo => uo.Id == id && uo.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership record details with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<UnitOwnership>, int)> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Retrieving paged ownership records. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                
                var query = _context.UnitOwnerships
                    .Include(uo => uo.Unit)
                        .ThenInclude(u => u.Floor)
                            .ThenInclude(f => f.Block)
                    .Include(uo => uo.Owner)
                    .Where(uo => uo.IsActive);
                
                var totalCount = await query.CountAsync();
                
                var ownerships = await query
                    .OrderByDescending(uo => uo.StartDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (ownerships, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged ownership records");
                throw;
            }
        }
    }
} 