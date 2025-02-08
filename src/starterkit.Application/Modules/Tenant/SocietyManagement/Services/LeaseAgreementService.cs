using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Persistence;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Service implementation for managing lease agreements
    /// </summary>
    public class LeaseAgreementService : ILeaseAgreementService
    {
        private readonly ILeaseAgreementRepository _leaseAgreementRepository;
        private readonly IUnitResidentRepository _unitResidentRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUnitOwnershipRepository _unitOwnershipRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITenantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateLeaseAgreementRequest> _createValidator;
        private readonly IValidator<UpdateLeaseAgreementRequest> _updateValidator;
        private readonly ILogger<LeaseAgreementService> _logger;

        public LeaseAgreementService(
            ILeaseAgreementRepository leaseAgreementRepository,
            IUnitResidentRepository unitResidentRepository,
            IUnitRepository unitRepository,
            IUnitOwnershipRepository unitOwnershipRepository,
            IUserRepository userRepository,
            ITenantDbContext dbContext,
            IMapper mapper,
            IValidator<CreateLeaseAgreementRequest> createValidator,
            IValidator<UpdateLeaseAgreementRequest> updateValidator,
            ILogger<LeaseAgreementService> logger)
        {
            _leaseAgreementRepository = leaseAgreementRepository;
            _unitResidentRepository = unitResidentRepository;
            _unitRepository = unitRepository;
            _unitOwnershipRepository = unitOwnershipRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetAllAsync()
        {
            try
            {
                var leaseAgreements = await _leaseAgreementRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<LeaseAgreementResponse>>(leaseAgreements);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all lease agreements");
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateError("Error retrieving lease agreements");
            }
        }

        public async Task<ApiResponse<LeaseAgreementResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var leaseAgreement = await _leaseAgreementRepository.GetByIdWithDetailsAsync(id);
                if (leaseAgreement == null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Lease agreement not found");

                var response = _mapper.Map<LeaseAgreementResponse>(leaseAgreement);
                return ApiResponse<LeaseAgreementResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lease agreement by ID {Id}", id);
                return ApiResponse<LeaseAgreementResponse>.CreateError("Error retrieving lease agreement");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                var leaseAgreements = await _leaseAgreementRepository.GetByUnitIdAsync(unitId);
                var response = _mapper.Map<IEnumerable<LeaseAgreementResponse>>(leaseAgreements);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lease agreements for unit {UnitId}", unitId);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateError("Error retrieving lease agreements");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                var leaseAgreements = await _leaseAgreementRepository.GetByOwnerIdAsync(ownerId);
                var response = _mapper.Map<IEnumerable<LeaseAgreementResponse>>(leaseAgreements);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lease agreements for owner {OwnerId}", ownerId);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateError("Error retrieving lease agreements");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaseAgreementResponse>>> GetByTenantIdAsync(Guid tenantId)
        {
            try
            {
                var leaseAgreements = await _leaseAgreementRepository.GetByTenantIdAsync(tenantId);
                var response = _mapper.Map<IEnumerable<LeaseAgreementResponse>>(leaseAgreements);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lease agreements for tenant {TenantId}", tenantId);
                return ApiResponse<IEnumerable<LeaseAgreementResponse>>.CreateError("Error retrieving lease agreements");
            }
        }

        public async Task<ApiResponse<LeaseAgreementResponse>> GetActiveLeaseForUnitAsync(Guid unitId)
        {
            try
            {
                var leaseAgreement = await _leaseAgreementRepository.GetActiveLeaseForUnitAsync(unitId);
                if (leaseAgreement == null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("No active lease agreement found for this unit");

                var response = _mapper.Map<LeaseAgreementResponse>(leaseAgreement);
                return ApiResponse<LeaseAgreementResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active lease agreement for unit {UnitId}", unitId);
                return ApiResponse<LeaseAgreementResponse>.CreateError("Error retrieving active lease agreement");
            }
        }

        public async Task<ApiResponse<PagedResponse<LeaseAgreementResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (items, totalCount) = await _leaseAgreementRepository.GetPagedAsync(pageNumber, pageSize);
                var mappedItems = _mapper.Map<IEnumerable<LeaseAgreementResponse>>(items);
                var response = new PagedResponse<LeaseAgreementResponse>(mappedItems, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<LeaseAgreementResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged lease agreements");
                return ApiResponse<PagedResponse<LeaseAgreementResponse>>.CreateError("Error retrieving paged lease agreements");
            }
        }

        public async Task<ApiResponse<LeaseAgreementResponse>> CreateAsync(CreateLeaseAgreementRequest request)
        {
            try
            {
                // 1. Request Validation
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<LeaseAgreementResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Unit Validation
                var unit = await _unitRepository.GetByIdAsync(request.UnitId);
                if (unit == null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Unit not found");

                if (!unit.IsActive)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Unit is not active");

                // 3. Owner Validation
                var currentOwnership = await _unitOwnershipRepository.GetCurrentOwnershipAsync(request.UnitId);
                _logger.LogInformation(
                    "Owner Validation - UnitId: {UnitId}, RequestOwnerId: {RequestOwnerId}, " +
                    "CurrentOwnership: {CurrentOwnership}, OwnershipActive: {OwnershipActive}, CurrentOwnerId: {CurrentOwnerId}",
                    request.UnitId,
                    request.OwnerId,
                    currentOwnership != null,
                    currentOwnership?.IsActive,
                    currentOwnership?.OwnerId);

                // Check if there is a current ownership record
                if (currentOwnership == null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("No ownership record found for this unit");

                // Check if the ownership is active
                if (!currentOwnership.IsActive)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("No active ownership record found for this unit");

                // Check if the requester is the current owner
                if (currentOwnership.OwnerId != request.OwnerId)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("The specified user is not the current owner of this unit");

                // 4. Tenant Validation
                var tenant = await _userRepository.GetByIdAsync(request.TenantId);
                if (tenant == null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Tenant not found");

                if (!tenant.IsActive)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Tenant account is not active");

                // 5. Check for existing active lease
                var existingLease = await _leaseAgreementRepository.GetActiveLeaseForUnitAsync(request.UnitId);
                if (existingLease != null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("An active lease agreement already exists for this unit");

                // 6. Check if tenant has other active leases
                var tenantActiveLeases = await _leaseAgreementRepository.GetByTenantIdAsync(request.TenantId);
                if (tenantActiveLeases.Any(l => l.Status == "Active" && l.EndDate > DateTime.UtcNow))
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Tenant already has an active lease agreement");

                // 7. Check for existing resident record
                var existingResident = await _unitResidentRepository.GetCurrentResidentsAsync(request.UnitId);
                if (existingResident.Any(r => r.ResidentId == request.TenantId))
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Tenant is already a resident of this unit");

                // 8. Validate dates
                if (request.StartDate < DateTime.UtcNow.Date)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Lease start date cannot be in the past");

                if (request.EndDate <= request.StartDate)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Lease end date must be after start date");

                try
                {
                    // Create lease agreement
                    var leaseAgreement = _mapper.Map<LeaseAgreement>(request);
                    leaseAgreement.Status = "Active";
                    var created = await _leaseAgreementRepository.AddAsync(leaseAgreement);

                    // Create unit resident record
                    var unitResident = new UnitResident
                    {
                        UnitId = request.UnitId,
                        ResidentId = request.TenantId,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        IsPrimary = true,
                        IsActive = true,
                        RelationType = "Tenant",
                        CreatedBy = created.CreatedBy
                    };
                    await _unitResidentRepository.AddAsync(unitResident);

                    var response = _mapper.Map<LeaseAgreementResponse>(created);
                    return ApiResponse<LeaseAgreementResponse>.CreateSuccess(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during lease agreement creation");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lease agreement for unit {UnitId} and tenant {TenantId}", 
                    request.UnitId, request.TenantId);
                return ApiResponse<LeaseAgreementResponse>.CreateError("Error creating lease agreement. Please try again later.");
            }
        }

        public async Task<ApiResponse<LeaseAgreementResponse>> UpdateAsync(UpdateLeaseAgreementRequest request)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<LeaseAgreementResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                var existing = await _leaseAgreementRepository.GetByIdAsync(request.Id);
                if (existing == null)
                    return ApiResponse<LeaseAgreementResponse>.CreateError("Lease agreement not found");

                // Update only allowed fields
                existing.EndDate = request.EndDate;
                existing.RentAmount = request.RentAmount;
                existing.PaymentFrequency = request.PaymentFrequency;
                existing.NoticePeriodDays = request.NoticePeriodDays;
                existing.Status = request.Status;

                var updated = await _leaseAgreementRepository.UpdateAsync(existing);
                var response = _mapper.Map<LeaseAgreementResponse>(updated);

                return ApiResponse<LeaseAgreementResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lease agreement {Id}", request.Id);
                return ApiResponse<LeaseAgreementResponse>.CreateError("Error updating lease agreement");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var deleted = await _leaseAgreementRepository.DeleteAsync(id);
                if (!deleted)
                    return ApiResponse<bool>.CreateError("Lease agreement not found");

                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lease agreement {Id}", id);
                return ApiResponse<bool>.CreateError("Error deleting lease agreement");
            }
        }

        public async Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request)
        {
            try
            {
                // Get data with optional filtering
                var query = await _leaseAgreementRepository.GetAllAsync();
                var filteredLeases = query.AsQueryable();

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredLeases = filteredLeases.Where(x => 
                        x.Unit.UnitNumber.ToLower().Contains(searchTerm) ||
                        x.Tenant.FullName.ToLower().Contains(searchTerm) ||
                        x.Status.ToLower().Contains(searchTerm));
                }

                // Apply sorting if provided
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var isAscending = string.IsNullOrWhiteSpace(request.SortDirection) || 
                                    request.SortDirection.ToLower() == "asc";

                    filteredLeases = request.SortBy.ToLower() switch
                    {
                        "unitnumber" => isAscending 
                            ? filteredLeases.OrderBy(x => x.Unit.UnitNumber)
                            : filteredLeases.OrderByDescending(x => x.Unit.UnitNumber),
                        "tenant" => isAscending 
                            ? filteredLeases.OrderBy(x => x.Tenant.FullName)
                            : filteredLeases.OrderByDescending(x => x.Tenant.FullName),
                        "status" => isAscending 
                            ? filteredLeases.OrderBy(x => x.Status)
                            : filteredLeases.OrderByDescending(x => x.Status),
                        _ => filteredLeases.OrderBy(x => x.Unit.UnitNumber)
                    };
                }

                // Convert to list for pagination
                var leasesList = filteredLeases.ToList();
                var totalCount = leasesList.Count;

                // Apply pagination if requested
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    var pageNumber = Math.Max(1, request.Page.Value);
                    var pageSize = Math.Max(1, request.PageSize.Value);
                    leasesList = leasesList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Map to lookup DTOs
                var lookupItems = leasesList.Select(x => new LookupDto
                {
                    Id = x.Id,
                    Label = $"Unit {x.Unit.UnitNumber} - {x.Tenant.FullName}",
                    Value = x.Id.ToString(),
                    Group = x.Status,
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "unitId", x.UnitId.ToString() },
                        { "unitNumber", x.Unit.UnitNumber },
                        { "tenantId", x.TenantId.ToString() },
                        { "tenantName", x.Tenant.FullName },
                        { "ownerId", x.OwnerId.ToString() },
                        { "ownerName", x.Owner.FullName },
                        { "status", x.Status },
                        { "startDate", x.StartDate.ToString("yyyy-MM-dd") },
                        { "endDate", x.EndDate.ToString("yyyy-MM-dd") }
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
                _logger.LogError(ex, "Error getting lease agreement lookup data");
                return ApiResponse<LookupResponse<LookupDto>>.CreateError("Error retrieving lease agreement lookup data");
            }
        }
    }
} 