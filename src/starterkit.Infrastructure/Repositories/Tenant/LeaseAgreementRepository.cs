using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Application.Persistence;

namespace starterkit.Infrastructure.Repositories.Tenant
{
    /// <summary>
    /// Repository implementation for managing LeaseAgreement entities.
    /// </summary>
    public class LeaseAgreementRepository : ILeaseAgreementRepository
    {
        private readonly ITenantDbContext _context;

        public LeaseAgreementRepository(ITenantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LeaseAgreement>> GetAllAsync()
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<LeaseAgreement> GetByIdAsync(Guid id)
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<IEnumerable<LeaseAgreement>> GetByUnitIdAsync(Guid unitId)
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .Where(x => x.UnitId == unitId && x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaseAgreement>> GetByOwnerIdAsync(Guid ownerId)
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .Where(x => x.OwnerId == ownerId && x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaseAgreement>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .ToListAsync();
        }

        public async Task<LeaseAgreement> GetActiveLeaseForUnitAsync(Guid unitId)
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .FirstOrDefaultAsync(x => x.UnitId == unitId 
                    && x.IsActive 
                    && x.Status == "Active" 
                    && x.StartDate <= DateTime.UtcNow 
                    && x.EndDate >= DateTime.UtcNow);
        }

        public async Task<(IEnumerable<LeaseAgreement> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                    .ThenInclude(o => o.Profile)
                .Include(x => x.Tenant)
                    .ThenInclude(t => t.Profile)
                .Include(x => x.RentPayments)
                .Where(x => x.IsActive);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<LeaseAgreement> AddAsync(LeaseAgreement leaseAgreement)
        {
            await _context.LeaseAgreements.AddAsync(leaseAgreement);
            await _context.SaveChangesAsync();
            return leaseAgreement;
        }

        public async Task<LeaseAgreement> UpdateAsync(LeaseAgreement leaseAgreement)
        {
            _context.LeaseAgreements.Update(leaseAgreement);
            await _context.SaveChangesAsync();
            return leaseAgreement;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var leaseAgreement = await GetByIdAsync(id);
            if (leaseAgreement == null) return false;

            leaseAgreement.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.LeaseAgreements
                .AnyAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<LeaseAgreement> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.LeaseAgreements
                .Include(x => x.Unit)
                .Include(x => x.Owner)
                .Include(x => x.Tenant)
                .Include(x => x.RentPayments)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }
    }
} 