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
    /// Implementation of ISocietyRepository.
    /// Handles data access operations for Society entities using Entity Framework Core.
    /// </summary>
    public class SocietyRepository : ISocietyRepository
    {
        private readonly ITenantDbContext _context;
        private readonly ILogger<SocietyRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the SocietyRepository class.
        /// </summary>
        /// <param name="context">The tenant database context.</param>
        /// <param name="logger">The logger instance for this repository.</param>
        public SocietyRepository(ITenantDbContext context, ILogger<SocietyRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Society>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all societies");
                return await _context.Societies
                    .Where(s => s.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all societies");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Society> GetByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving society with ID: {Id}", id);
                return await _context.Societies
                    .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Society> GetByRegistrationNumberAsync(string registrationNumber)
        {
            try
            {
                _logger.LogInformation("Retrieving society with registration number: {RegistrationNumber}", registrationNumber);
                return await _context.Societies
                    .FirstOrDefaultAsync(s => s.RegistrationNumber == registrationNumber && s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society with registration number: {RegistrationNumber}", registrationNumber);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Society> AddAsync(Society society)
        {
            try
            {
                _logger.LogInformation("Adding new society: {Name}", society.Name);
                await _context.Societies.AddAsync(society);
                await _context.SaveChangesAsync();
                return society;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding society: {Name}", society.Name);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Society> UpdateAsync(Society society)
        {
            try
            {
                _logger.LogInformation("Updating society with ID: {Id}", society.Id);
                _context.Societies.Update(society);
                await _context.SaveChangesAsync();
                return society;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating society with ID: {Id}", society.Id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Checking if society exists with ID: {Id}", id);
                return await _context.Societies
                    .AnyAsync(s => s.Id == id && s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of society with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber)
        {
            try
            {
                _logger.LogInformation("Checking if society exists with registration number: {RegistrationNumber}", registrationNumber);
                return await _context.Societies
                    .AnyAsync(s => s.RegistrationNumber == registrationNumber && s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of society with registration number: {RegistrationNumber}", registrationNumber);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Soft deleting society with ID: {Id}", id);
                var society = await _context.Societies.FindAsync(id);
                if (society == null || !society.IsActive)
                    return false;

                society.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting society with ID: {Id}", id);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Society>> GetAllWithDetailsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all societies with details");
                return await _context.Societies
                    .Include(s => s.Address)
                    .Include(s => s.Blocks)
                        .ThenInclude(b => b.Floors)
                            .ThenInclude(f => f.Units)
                    .Include(s => s.Subscriptions)
                    .Include(s => s.FeatureAccess)
                    .Where(s => s.IsActive)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all societies with details");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<Society> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Retrieving society with details for ID: {Id}", id);
                return await _context.Societies
                    .Include(s => s.Address)
                    .Include(s => s.Blocks)
                        .ThenInclude(b => b.Floors)
                            .ThenInclude(f => f.Units)
                    .Include(s => s.Subscriptions)
                    .Include(s => s.FeatureAccess)
                    .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society with details for ID: {Id}", id);
                throw;
            }
        }
    }
} 