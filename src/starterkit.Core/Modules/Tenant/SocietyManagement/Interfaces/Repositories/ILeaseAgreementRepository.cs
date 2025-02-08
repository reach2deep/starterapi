using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing LeaseAgreement entities.
    /// Provides data access operations for lease agreements.
    /// </summary>
    public interface ILeaseAgreementRepository
    {
        /// <summary>
        /// Retrieves all lease agreements asynchronously.
        /// </summary>
        Task<IEnumerable<LeaseAgreement>> GetAllAsync();

        /// <summary>
        /// Retrieves a lease agreement by its unique identifier.
        /// </summary>
        Task<LeaseAgreement> GetByIdAsync(Guid id);

        /// <summary>
        /// Retrieves all lease agreements for a specific unit.
        /// </summary>
        Task<IEnumerable<LeaseAgreement>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Retrieves all lease agreements for a specific owner.
        /// </summary>
        Task<IEnumerable<LeaseAgreement>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Retrieves all lease agreements for a specific tenant.
        /// </summary>
        Task<IEnumerable<LeaseAgreement>> GetByTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Gets the active lease agreement for a specific unit.
        /// </summary>
        Task<LeaseAgreement> GetActiveLeaseForUnitAsync(Guid unitId);

        /// <summary>
        /// Gets a paged list of lease agreements.
        /// </summary>
        Task<(IEnumerable<LeaseAgreement> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Adds a new lease agreement.
        /// </summary>
        Task<LeaseAgreement> AddAsync(LeaseAgreement leaseAgreement);

        /// <summary>
        /// Updates an existing lease agreement.
        /// </summary>
        Task<LeaseAgreement> UpdateAsync(LeaseAgreement leaseAgreement);

        /// <summary>
        /// Soft deletes a lease agreement.
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Checks if a lease agreement exists.
        /// </summary>
        Task<bool> ExistsAsync(Guid id);

        /// <summary>
        /// Gets a lease agreement with all related entities loaded.
        /// </summary>
        Task<LeaseAgreement> GetByIdWithDetailsAsync(Guid id);
    }
} 