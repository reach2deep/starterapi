using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing lease agreements
    /// </summary>
    public interface ILeaseAgreementService
    {
        /// <summary>
        /// Gets all lease agreements
        /// </summary>
        Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetAllAsync();

        /// <summary>
        /// Gets a lease agreement by ID
        /// </summary>
        Task<ApiResponse<LeaseAgreementResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all lease agreements for a specific unit
        /// </summary>
        Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetByUnitIdAsync(Guid unitId);

        /// <summary>
        /// Gets all lease agreements for a specific owner
        /// </summary>
        Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Gets all lease agreements for a specific tenant
        /// </summary>
        Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetByTenantIdAsync(Guid tenantId);

        /// <summary>
        /// Gets the active lease agreement for a specific unit
        /// </summary>
        Task<ApiResponse<LeaseAgreementResponse>> GetActiveLeaseForUnitAsync(Guid unitId);

        /// <summary>
        /// Gets a paged list of lease agreements
        /// </summary>
        Task<ApiResponse<PagedResponse<LeaseAgreementResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new lease agreement
        /// </summary>
        Task<ApiResponse<LeaseAgreementResponse>> CreateAsync(CreateLeaseAgreementRequest request);

        /// <summary>
        /// Updates an existing lease agreement
        /// </summary>
        Task<ApiResponse<LeaseAgreementResponse>> UpdateAsync(UpdateLeaseAgreementRequest request);

        /// <summary>
        /// Deletes a lease agreement
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Gets lease agreements data optimized for dropdown/lookup controls
        /// </summary>
        Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request);
    }
} 