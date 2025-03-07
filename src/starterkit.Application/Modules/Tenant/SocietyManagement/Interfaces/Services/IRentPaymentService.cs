using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing rent payments
    /// </summary>
    public interface IRentPaymentService
    {
        /// <summary>
        /// Gets all rent payments
        /// </summary>
        Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetAllAsync();

        /// <summary>
        /// Gets a rent payment by ID
        /// </summary>
        Task<ApiResponse<RentPaymentResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all rent payments for a specific lease agreement
        /// </summary>
        Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetByLeaseAgreementIdAsync(Guid leaseAgreementId);

        /// <summary>
        /// Gets all payments with a specific status
        /// </summary>
        Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetByStatusAsync(string status);

        /// <summary>
        /// Gets all payments due between specific dates
        /// </summary>
        Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all overdue payments
        /// </summary>
        Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetOverduePaymentsAsync();

        /// <summary>
        /// Gets a paged list of rent payments
        /// </summary>
        Task<ApiResponse<PagedResponse<RentPaymentResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new rent payment
        /// </summary>
        Task<ApiResponse<RentPaymentResponse>> CreateAsync(CreateRentPaymentRequest request);

        /// <summary>
        /// Creates a new rent payment for a specific unit's active lease
        /// </summary>
        Task<ApiResponse<RentPaymentResponse>> CreateByUnitIdAsync(Guid unitId, CreateRentPaymentByUnitRequest request);

        /// <summary>
        /// Updates an existing rent payment
        /// </summary>
        Task<ApiResponse<RentPaymentResponse>> UpdateAsync(UpdateRentPaymentRequest request);

        /// <summary>
        /// Deletes a rent payment
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Gets rent payments data optimized for dropdown/lookup controls
        /// </summary>
        Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request);
    }
} 