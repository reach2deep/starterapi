using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing RentPayment entities.
    /// Provides data access operations for rent payments.
    /// </summary>
    public interface IRentPaymentRepository
    {
        /// <summary>
        /// Retrieves all rent payments asynchronously.
        /// </summary>
        Task<IEnumerable<RentPayment>> GetAllAsync();

        /// <summary>
        /// Retrieves a rent payment by its unique identifier.
        /// </summary>
        Task<RentPayment> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all rent payments for a specific lease agreement.
        /// </summary>
        Task<IEnumerable<RentPayment>> GetByLeaseAgreementIdAsync(Guid leaseAgreementId);

        /// <summary>
        /// Gets all payments with a specific status.
        /// </summary>
        Task<IEnumerable<RentPayment>> GetByStatusAsync(string status);

        /// <summary>
        /// Gets all payments due between specific dates.
        /// </summary>
        Task<IEnumerable<RentPayment>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all overdue payments.
        /// </summary>
        Task<IEnumerable<RentPayment>> GetOverduePaymentsAsync();

        /// <summary>
        /// Gets a paged list of rent payments.
        /// </summary>
        Task<(IEnumerable<RentPayment> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new rent payment.
        /// </summary>
        Task<RentPayment> AddAsync(RentPayment rentPayment);

        /// <summary>
        /// Updates an existing rent payment.
        /// </summary>
        Task<RentPayment> UpdateAsync(RentPayment rentPayment);

        /// <summary>
        /// Soft deletes a rent payment.
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a rent payment exists.
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Gets a rent payment with all related entities loaded.
        /// </summary>
        Task<RentPayment> GetByIdWithDetailsAsync(Guid id);
    }
} 