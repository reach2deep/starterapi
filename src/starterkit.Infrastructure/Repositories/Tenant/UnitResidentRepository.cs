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
    /// Implementation of IUnitResidentRepository.
    /// Handles data access operations for UnitResident entities using Entity Framework Core.
    /// </summary>
    public class UnitResidentRepository : IUnitResidentRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<UnitResidentRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the UnitResidentRepository class.
        /// </summary>
        /// <param name="context">The tenant database context.</param>
        /// <param name="logger">The logger instance for this repository.</param>
        public UnitResidentRepository(ITenantDbContext context, ILogger<UnitResidentRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitResident>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all unit resident records");
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .Where(ur => ur.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all unit resident records");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitResident>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                _logger.LogInformation("Retrieving resident records for unit ID: {UnitId}", unitId);
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .Where(ur => ur.UnitId == unitId && ur.IsActive)
                    .OrderByDescending(ur => ur.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident records for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitResident>> GetByResidentIdAsync(Guid residentId)
        {
            try
            {
                _logger.LogInformation("Retrieving resident records for resident ID: {ResidentId}", residentId);
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .Where(ur => ur.ResidentId == residentId && ur.IsActive)
                    .OrderByDescending(ur => ur.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident records for resident ID: {ResidentId}", residentId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<UnitResident>> GetCurrentResidentsAsync(Guid unitId)
        {
            try
            {
                _logger.LogInformation("Retrieving current resident records for unit ID: {UnitId}", unitId);
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .Where(ur => ur.UnitId == unitId && 
                                ur.IsActive && 
                                ur.EndDate == null)
                    .OrderByDescending(ur => ur.StartDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current resident records for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitResident> GetPrimaryResidentAsync(Guid unitId)
        {
            try
            {
                _logger.LogInformation("Retrieving primary resident record for unit ID: {UnitId}", unitId);
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .Where(ur => ur.UnitId == unitId && 
                                ur.IsActive && 
                                ur.EndDate == null && 
                                ur.IsPrimary)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving primary resident record for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitResident> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving resident record with ID: {Id}", id);
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .FirstOrDefaultAsync(ur => ur.Id == id && ur.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident record with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitResident> AddAsync(UnitResident resident)
        {
            try
            {
                _logger.LogInformation("Adding new resident record for unit: {UnitId} and resident: {ResidentId}", 
                    resident.UnitId, resident.ResidentId);
                await _context.UnitResidents.AddAsync(resident);
                await _context.SaveChangesAsync();
                return resident;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding resident record");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitResident> UpdateAsync(UnitResident resident)
        {
            try
            {
                _logger.LogInformation("Updating resident record with ID: {Id}", resident.Id);
                _context.UnitResidents.Update(resident);
                await _context.SaveChangesAsync();
                return resident;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating resident record with ID: {Id}", resident.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Checking if resident record exists with ID: {Id}", id);
                return await _context.UnitResidents
                    .AnyAsync(ur => ur.Id == id && ur.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of resident record with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> HasActiveResidentsAsync(Guid unitId)
        {
            try
            {
                _logger.LogInformation("Checking if unit has active residents: {UnitId}", unitId);
                return await _context.UnitResidents
                    .AnyAsync(ur => ur.UnitId == unitId && 
                                  ur.IsActive && 
                                  ur.EndDate == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking active residents for unit ID: {UnitId}", unitId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> IsActiveResidentAsync(Guid residentId)
        {
            try
            {
                _logger.LogInformation("Checking if resident is active: {ResidentId}", residentId);
                return await _context.UnitResidents
                    .AnyAsync(ur => ur.ResidentId == residentId && 
                                  ur.IsActive && 
                                  ur.EndDate == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if resident is active: {ResidentId}", residentId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Soft deleting resident record with ID: {Id}", id);
                var resident = await _context.UnitResidents.FindAsync(id);
                if (resident == null || !resident.IsActive)
                    return false;

                resident.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting resident record with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<UnitResident> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving resident record with details for ID: {Id}", id);
                return await _context.UnitResidents
                    .Include(ur => ur.Unit)
                        .ThenInclude(u => u.Floor)
                            .ThenInclude(f => f.Block)
                                .ThenInclude(b => b.Society)
                    .Include(ur => ur.Resident)
                    .FirstOrDefaultAsync(ur => ur.Id == id && ur.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving resident record with details for ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(IEnumerable<UnitResident>, int)> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                _logger.LogInformation("Retrieving paged resident records. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                
                var query = _context.UnitResidents
                    .Include(ur => ur.Unit)
                    .Include(ur => ur.Resident)
                        .ThenInclude(r => r.Profile)
                    .Where(ur => ur.IsActive);
                
                var totalCount = await query.CountAsync();
                
                var residents = await query
                    .OrderByDescending(ur => ur.StartDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (residents, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged resident records");
                throw;
            }
        }
    }
} 