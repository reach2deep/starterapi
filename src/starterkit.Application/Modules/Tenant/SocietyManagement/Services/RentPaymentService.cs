using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Service implementation for managing rent payments
    /// </summary>
    public class RentPaymentService : IRentPaymentService
    {
        private readonly IRentPaymentRepository _rentPaymentRepository;
        private readonly ILeaseAgreementRepository _leaseAgreementRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRentPaymentRequest> _createValidator;
        private readonly IValidator<UpdateRentPaymentRequest> _updateValidator;
        private readonly ILogger<RentPaymentService> _logger;

        public RentPaymentService(
            IRentPaymentRepository rentPaymentRepository,
            ILeaseAgreementRepository leaseAgreementRepository,
            IMapper mapper,
            IValidator<CreateRentPaymentRequest> createValidator,
            IValidator<UpdateRentPaymentRequest> updateValidator,
            ILogger<RentPaymentService> logger)
        {
            _rentPaymentRepository = rentPaymentRepository;
            _leaseAgreementRepository = leaseAgreementRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetAllAsync()
        {
            try
            {
                var rentPayments = await _rentPaymentRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<RentPaymentResponse>>(rentPayments);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all rent payments");
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateError("Error retrieving rent payments");
            }
        }

        public async Task<ApiResponse<RentPaymentResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var rentPayment = await _rentPaymentRepository.GetByIdWithDetailsAsync(id);
                if (rentPayment == null)
                    return ApiResponse<RentPaymentResponse>.CreateError("Rent payment not found");

                var response = _mapper.Map<RentPaymentResponse>(rentPayment);
                return ApiResponse<RentPaymentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rent payment by ID {Id}", id);
                return ApiResponse<RentPaymentResponse>.CreateError("Error retrieving rent payment");
            }
        }

        public async Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetByLeaseAgreementIdAsync(Guid leaseAgreementId)
        {
            try
            {
                var rentPayments = await _rentPaymentRepository.GetByLeaseAgreementIdAsync(leaseAgreementId);
                var response = _mapper.Map<IEnumerable<RentPaymentResponse>>(rentPayments);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rent payments for lease agreement {LeaseAgreementId}", leaseAgreementId);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateError("Error retrieving rent payments");
            }
        }

        public async Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetByStatusAsync(string status)
        {
            try
            {
                var rentPayments = await _rentPaymentRepository.GetByStatusAsync(status);
                var response = _mapper.Map<IEnumerable<RentPaymentResponse>>(rentPayments);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rent payments by status {Status}", status);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateError("Error retrieving rent payments");
            }
        }

        public async Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var rentPayments = await _rentPaymentRepository.GetByDueDateRangeAsync(startDate, endDate);
                var response = _mapper.Map<IEnumerable<RentPaymentResponse>>(rentPayments);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rent payments for date range {StartDate} to {EndDate}", startDate, endDate);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateError("Error retrieving rent payments");
            }
        }

        public async Task<ApiResponse<IEnumerable<RentPaymentResponse>>> GetOverduePaymentsAsync()
        {
            try
            {
                var rentPayments = await _rentPaymentRepository.GetOverduePaymentsAsync();
                var response = _mapper.Map<IEnumerable<RentPaymentResponse>>(rentPayments);
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting overdue rent payments");
                return ApiResponse<IEnumerable<RentPaymentResponse>>.CreateError("Error retrieving overdue rent payments");
            }
        }

        public async Task<ApiResponse<PagedResponse<RentPaymentResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (items, totalCount) = await _rentPaymentRepository.GetPagedAsync(pageNumber, pageSize);
                var mappedItems = _mapper.Map<IEnumerable<RentPaymentResponse>>(items);
                var response = new PagedResponse<RentPaymentResponse>(mappedItems, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<RentPaymentResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged rent payments");
                return ApiResponse<PagedResponse<RentPaymentResponse>>.CreateError("Error retrieving paged rent payments");
            }
        }

        public async Task<ApiResponse<RentPaymentResponse>> CreateAsync(CreateRentPaymentRequest request)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<RentPaymentResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // Verify lease agreement exists and is active
                var leaseAgreement = await _leaseAgreementRepository.GetByIdAsync(request.LeaseAgreementId);
                if (leaseAgreement == null)
                    return ApiResponse<RentPaymentResponse>.CreateError("Lease agreement not found");

                if (leaseAgreement.Status != "Active")
                    return ApiResponse<RentPaymentResponse>.CreateError("Cannot create payment for inactive lease agreement");

                var rentPayment = _mapper.Map<RentPayment>(request);
                var created = await _rentPaymentRepository.AddAsync(rentPayment);
                var response = _mapper.Map<RentPaymentResponse>(created);

                return ApiResponse<RentPaymentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating rent payment");
                return ApiResponse<RentPaymentResponse>.CreateError("Error creating rent payment");
            }
        }

        public async Task<ApiResponse<RentPaymentResponse>> UpdateAsync(UpdateRentPaymentRequest request)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<RentPaymentResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                var existing = await _rentPaymentRepository.GetByIdAsync(request.Id);
                if (existing == null)
                    return ApiResponse<RentPaymentResponse>.CreateError("Rent payment not found");

                // Update only allowed fields
                existing.PaidDate = request.PaidDate;
                existing.Amount = request.Amount;
                existing.PaymentMode = request.PaymentMode;
                existing.Status = request.Status;
                existing.TransactionReference = request.TransactionReference;

                var updated = await _rentPaymentRepository.UpdateAsync(existing);
                var response = _mapper.Map<RentPaymentResponse>(updated);

                return ApiResponse<RentPaymentResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating rent payment {Id}", request.Id);
                return ApiResponse<RentPaymentResponse>.CreateError("Error updating rent payment");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var deleted = await _rentPaymentRepository.DeleteAsync(id);
                if (!deleted)
                    return ApiResponse<bool>.CreateError("Rent payment not found");

                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting rent payment {Id}", id);
                return ApiResponse<bool>.CreateError("Error deleting rent payment");
            }
        }

        public async Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request)
        {
            try
            {
                // Get data with optional filtering
                var query = await _rentPaymentRepository.GetAllAsync();
                var filteredPayments = query.AsQueryable();

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredPayments = filteredPayments.Where(x => 
                        x.LeaseAgreement.Unit.UnitNumber.ToLower().Contains(searchTerm) ||
                        x.LeaseAgreement.Tenant.FullName.ToLower().Contains(searchTerm) ||
                        x.Status.ToLower().Contains(searchTerm) ||
                        x.PaymentMode.ToLower().Contains(searchTerm));
                }

                // Apply sorting if provided
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var isAscending = string.IsNullOrWhiteSpace(request.SortDirection) || 
                                    request.SortDirection.ToLower() == "asc";

                    filteredPayments = request.SortBy.ToLower() switch
                    {
                        "duedate" => isAscending 
                            ? filteredPayments.OrderBy(x => x.DueDate)
                            : filteredPayments.OrderByDescending(x => x.DueDate),
                        "paiddate" => isAscending 
                            ? filteredPayments.OrderBy(x => x.PaidDate)
                            : filteredPayments.OrderByDescending(x => x.PaidDate),
                        "amount" => isAscending 
                            ? filteredPayments.OrderBy(x => x.Amount)
                            : filteredPayments.OrderByDescending(x => x.Amount),
                        "status" => isAscending 
                            ? filteredPayments.OrderBy(x => x.Status)
                            : filteredPayments.OrderByDescending(x => x.Status),
                        _ => filteredPayments.OrderByDescending(x => x.DueDate)
                    };
                }

                // Convert to list for pagination
                var paymentsList = filteredPayments.ToList();
                var totalCount = paymentsList.Count;

                // Apply pagination if requested
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    var pageNumber = Math.Max(1, request.Page.Value);
                    var pageSize = Math.Max(1, request.PageSize.Value);
                    paymentsList = paymentsList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Map to lookup DTOs
                var lookupItems = paymentsList.Select(x => new LookupDto
                {
                    Id = x.Id,
                    Label = $"Payment for Unit {x.LeaseAgreement.Unit.UnitNumber} - {x.DueDate:MMM yyyy}",
                    Value = x.Id.ToString(),
                    Group = x.Status,
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "leaseAgreementId", x.LeaseAgreementId.ToString() },
                        { "unitNumber", x.LeaseAgreement.Unit.UnitNumber },
                        { "tenantName", x.LeaseAgreement.Tenant.FullName },
                        { "dueDate", x.DueDate.ToString("yyyy-MM-dd") },
                        { "paidDate", x.PaidDate?.ToString("yyyy-MM-dd") ?? "" },
                        { "amount", x.Amount.ToString("F2") },
                        { "paymentMode", x.PaymentMode },
                        { "transactionReference", x.TransactionReference },
                        { "status", x.Status }
                    }
                });

                var response = new LookupResponse<LookupDto>
                {
                    Items = lookupItems,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize
                };

                return ApiResponse<LookupResponse<LookupDto>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting rent payment lookup data");
                return ApiResponse<LookupResponse<LookupDto>>.CreateError("Error retrieving rent payment lookup data");
            }
        }
    }
} 