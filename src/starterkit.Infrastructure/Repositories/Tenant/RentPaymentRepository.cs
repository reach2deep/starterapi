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
    /// Repository implementation for managing RentPayment entities.
    /// </summary>
    public class RentPaymentRepository : IRentPaymentRepository
    {
        private readonly ITenantDbContext _context;

        public RentPaymentRepository(ITenantDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RentPayment>> GetAllAsync()
        {
            return await _context.RentPayments
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        public async Task<RentPayment> GetByIdAsync(Guid id)
        {
            return await _context.RentPayments
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<IEnumerable<RentPayment>> GetByLeaseAgreementIdAsync(Guid leaseAgreementId)
        {
            return await _context.RentPayments
                .Where(x => x.LeaseAgreementId == leaseAgreementId && x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentPayment>> GetByStatusAsync(string status)
        {
            return await _context.RentPayments
                .Where(x => x.Status == status && x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentPayment>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.RentPayments
                .Where(x => x.DueDate >= startDate && x.DueDate <= endDate && x.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<RentPayment>> GetOverduePaymentsAsync()
        {
            return await _context.RentPayments
                .Where(x => x.DueDate < DateTime.UtcNow 
                    && x.Status != "Paid" 
                    && x.IsActive)
                .ToListAsync();
        }

        public async Task<(IEnumerable<RentPayment> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.RentPayments
                .Include(x => x.LeaseAgreement)
                    .ThenInclude(x => x.Unit)
                .Include(x => x.LeaseAgreement)
                    .ThenInclude(x => x.Tenant)
                        .ThenInclude(t => t.Profile)
                .Where(x => x.IsActive);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<RentPayment> AddAsync(RentPayment rentPayment)
        {
            await _context.RentPayments.AddAsync(rentPayment);
            await _context.SaveChangesAsync();
            return rentPayment;
        }

        public async Task<RentPayment> UpdateAsync(RentPayment rentPayment)
        {
            _context.RentPayments.Update(rentPayment);
            await _context.SaveChangesAsync();
            return rentPayment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var rentPayment = await GetByIdAsync(id);
            if (rentPayment == null) return false;

            rentPayment.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.RentPayments
                .AnyAsync(x => x.Id == id && x.IsActive);
        }

        public async Task<RentPayment> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.RentPayments
                .Include(x => x.LeaseAgreement)
                    .ThenInclude(x => x.Unit)
                .FirstOrDefaultAsync(x => x.Id == id && x.IsActive);
        }
    }
} 