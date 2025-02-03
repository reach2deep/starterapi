using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FluentValidation;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Tenant.SocietyManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Implementation of IUnitOwnershipService
    /// </summary>
    public class UnitOwnershipService : IUnitOwnershipService
    {
        private readonly IUnitOwnershipRepository _ownershipRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitOwnershipService> _logger;
        private readonly IValidator<CreateUnitOwnershipRequest> _createValidator;
        private readonly IValidator<UpdateUnitOwnershipRequest> _updateValidator;

        /// <summary>
        /// Initializes a new instance of the UnitOwnershipService class.
        /// </summary>
        /// <param name="ownershipRepository">The unit ownership repository.</param>
        /// <param name="unitRepository">The unit repository.</param>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="createValidator">The create validator instance.</param>
        /// <param name="updateValidator">The update validator instance.</param>
        public UnitOwnershipService(
            IUnitOwnershipRepository ownershipRepository,
            IUnitRepository unitRepository,
            IMapper mapper,
            ILogger<UnitOwnershipService> logger,
            IValidator<CreateUnitOwnershipRequest> createValidator,
            IValidator<UpdateUnitOwnershipRequest> updateValidator)
        {
            _ownershipRepository = ownershipRepository ?? throw new ArgumentNullException(nameof(ownershipRepository));
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitOwnershipResponse>>> GetAllAsync()
        {
            try
            {
                var ownerships = await _ownershipRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<UnitOwnershipResponse>>(ownerships);
                return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all ownership records");
                return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateError("Failed to retrieve ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitOwnershipResponse>>> GetByUnitIdAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateError("Unit not found");

                var ownerships = await _ownershipRepository.GetByUnitIdAsync(unitId);
                var response = _mapper.Map<IEnumerable<UnitOwnershipResponse>>(ownerships);
                return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership records for unit ID: {UnitId}", unitId);
                return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateError("Failed to retrieve ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitOwnershipResponse>>> GetByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                var ownerships = await _ownershipRepository.GetByOwnerIdAsync(ownerId);
                var response = _mapper.Map<IEnumerable<UnitOwnershipResponse>>(ownerships);
                return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership records for owner ID: {OwnerId}", ownerId);
                return ApiResponse<IEnumerable<UnitOwnershipResponse>>.CreateError("Failed to retrieve ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnershipResponse>> GetCurrentOwnershipAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Unit not found");

                var ownership = await _ownershipRepository.GetCurrentOwnershipAsync(unitId);
                if (ownership == null)
                    return ApiResponse<UnitOwnershipResponse>.CreateError("No current ownership record found");

                var response = _mapper.Map<UnitOwnershipResponse>(ownership);
                return ApiResponse<UnitOwnershipResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current ownership for unit ID: {UnitId}", unitId);
                return ApiResponse<UnitOwnershipResponse>.CreateError("Failed to retrieve current ownership");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnershipResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var ownership = await _ownershipRepository.GetByIdAsync(id);
                if (ownership == null)
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Ownership record not found");

                var response = _mapper.Map<UnitOwnershipResponse>(ownership);
                return ApiResponse<UnitOwnershipResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership record with ID: {Id}", id);
                return ApiResponse<UnitOwnershipResponse>.CreateError("Failed to retrieve ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnershipResponse>> CreateAsync(CreateUnitOwnershipRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UnitOwnershipResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                if (!await _unitRepository.ExistsAsync(request.UnitId))
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Unit not found");

                if (await _ownershipRepository.HasActiveOwnershipAsync(request.UnitId))
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Unit already has an active owner");

                // 3. Map to entity
                var ownership = _mapper.Map<UnitOwnership>(request);

                // 4. Save to database
                var createdOwnership = await _ownershipRepository.AddAsync(ownership);

                // 5. Return response
                var response = _mapper.Map<UnitOwnershipResponse>(createdOwnership);
                return ApiResponse<UnitOwnershipResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ownership record for unit ID: {UnitId}", request.UnitId);
                return ApiResponse<UnitOwnershipResponse>.CreateError("Failed to create ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnershipResponse>> UpdateAsync(UpdateUnitOwnershipRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UnitOwnershipResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                var existingOwnership = await _ownershipRepository.GetByIdAsync(request.Id);
                if (existingOwnership == null)
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Ownership record not found");

                if (!await _unitRepository.ExistsAsync(request.UnitId))
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Unit not found");

                // 3. Map to entity
                _mapper.Map(request, existingOwnership);

                // 4. Save to database
                var updatedOwnership = await _ownershipRepository.UpdateAsync(existingOwnership);

                // 5. Return response
                var response = _mapper.Map<UnitOwnershipResponse>(updatedOwnership);
                return ApiResponse<UnitOwnershipResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ownership record with ID: {Id}", request.Id);
                return ApiResponse<UnitOwnershipResponse>.CreateError("Failed to update ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _ownershipRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Ownership record not found");

                var result = await _ownershipRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ownership record with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete ownership record");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> ExistsAsync(Guid id)
        {
            try
            {
                var exists = await _ownershipRepository.ExistsAsync(id);
                return ApiResponse<bool>.CreateSuccess(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking existence of ownership record with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to check ownership record existence");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> HasActiveOwnershipAsync(Guid unitId)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<bool>.CreateError("Unit not found");

                var hasActiveOwnership = await _ownershipRepository.HasActiveOwnershipAsync(unitId);
                return ApiResponse<bool>.CreateSuccess(hasActiveOwnership);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking active ownership for unit ID: {UnitId}", unitId);
                return ApiResponse<bool>.CreateError("Failed to check active ownership");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnershipResponse>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var ownership = await _ownershipRepository.GetByIdWithDetailsAsync(id);
                if (ownership == null)
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Ownership record not found");

                var response = _mapper.Map<UnitOwnershipResponse>(ownership);
                return ApiResponse<UnitOwnershipResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving ownership record details with ID: {Id}", id);
                return ApiResponse<UnitOwnershipResponse>.CreateError("Failed to retrieve ownership record details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitOwnershipResponse>> TransferOwnershipAsync(Guid unitId, Guid currentOwnerId, Guid newOwnerId, DateTime transferDate)
        {
            try
            {
                // 1. Business rules
                if (!await _unitRepository.ExistsAsync(unitId))
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Unit not found");

                var currentOwnership = await _ownershipRepository.GetCurrentOwnershipAsync(unitId);
                if (currentOwnership == null)
                    return ApiResponse<UnitOwnershipResponse>.CreateError("No current ownership record found");

                if (currentOwnership.OwnerId != currentOwnerId)
                    return ApiResponse<UnitOwnershipResponse>.CreateError("Current owner ID does not match the unit's current owner");

                // 2. End current ownership
                currentOwnership.EndDate = transferDate;
                await _ownershipRepository.UpdateAsync(currentOwnership);

                // 3. Create new ownership
                var newOwnership = new UnitOwnership
                {
                    UnitId = unitId,
                    OwnerId = newOwnerId,
                    StartDate = transferDate,
                    Status = "Active"
                };

                var createdOwnership = await _ownershipRepository.AddAsync(newOwnership);

                // 4. Return response
                var response = _mapper.Map<UnitOwnershipResponse>(createdOwnership);
                return ApiResponse<UnitOwnershipResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while transferring ownership of unit ID: {UnitId} from owner ID: {CurrentOwnerId} to owner ID: {NewOwnerId}",
                    unitId, currentOwnerId, newOwnerId);
                return ApiResponse<UnitOwnershipResponse>.CreateError("Failed to transfer ownership");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<PagedResponse<UnitOwnershipResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (ownerships, totalCount) = await _ownershipRepository.GetPagedAsync(pageNumber, pageSize);
                var mappedOwnerships = _mapper.Map<IEnumerable<UnitOwnershipResponse>>(ownerships);
                var response = new PagedResponse<UnitOwnershipResponse>(mappedOwnerships, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<UnitOwnershipResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged ownership records");
                return ApiResponse<PagedResponse<UnitOwnershipResponse>>.CreateError("Failed to retrieve paged ownership records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request)
        {
            try
            {
                // Get all ownership records
                var ownerships = await _ownershipRepository.GetAllAsync();
                
                // Start with base query
                var filteredOwnerships = ownerships.AsQueryable();

                // Apply active filter unless specifically requested to include inactive
                if (!request.IncludeInactive)
                {
                    filteredOwnerships = filteredOwnerships.Where(o => o.Status.ToLower() == "active");
                }

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredOwnerships = filteredOwnerships.Where(o => 
                        (o.Unit != null && o.Unit.UnitNumber.ToLower().Contains(searchTerm)) ||
                        o.Status.ToLower().Contains(searchTerm));
                }

                // Apply optional filters if provided
                if (request.Filters?.Any() == true)
                {
                    foreach (var filter in request.Filters)
                    {
                        if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                        switch (filter.Key.ToLower())
                        {
                            case "unitid":
                                if (Guid.TryParse(filter.Value, out var unitId))
                                {
                                    filteredOwnerships = filteredOwnerships.Where(o => o.UnitId == unitId);
                                }
                                break;
                            case "ownerid":
                                if (Guid.TryParse(filter.Value, out var ownerId))
                                {
                                    filteredOwnerships = filteredOwnerships.Where(o => o.OwnerId == ownerId);
                                }
                                break;
                            case "status":
                                filteredOwnerships = filteredOwnerships.Where(o => o.Status.ToLower() == filter.Value.ToLower());
                                break;
                        }
                    }
                }

                // Apply sorting if requested
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var isAscending = string.IsNullOrWhiteSpace(request.SortDirection) || 
                                    request.SortDirection.ToLower() == "asc";

                    filteredOwnerships = request.SortBy.ToLower() switch
                    {
                        "startdate" => isAscending 
                            ? filteredOwnerships.OrderBy(o => o.StartDate)
                            : filteredOwnerships.OrderByDescending(o => o.StartDate),
                        "enddate" => isAscending 
                            ? filteredOwnerships.OrderBy(o => o.EndDate)
                            : filteredOwnerships.OrderByDescending(o => o.EndDate),
                        "status" => isAscending 
                            ? filteredOwnerships.OrderBy(o => o.Status)
                            : filteredOwnerships.OrderByDescending(o => o.Status),
                        _ => filteredOwnerships.OrderByDescending(o => o.StartDate) // Default sort by start date desc
                    };
                }

                // Convert to list for pagination
                var ownershipsList = filteredOwnerships.ToList();
                var totalCount = ownershipsList.Count;

                // Apply pagination if requested
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    var pageNumber = Math.Max(1, request.Page.Value);
                    var pageSize = Math.Max(1, request.PageSize.Value);
                    ownershipsList = ownershipsList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Map to lookup DTOs
                var lookupItems = ownershipsList.Select(o => new LookupDto
                {
                    Id = o.Id,
                    Label = $"Unit {o.Unit?.UnitNumber} - {(o.EndDate.HasValue ? "Previous" : "Current")} Owner",
                    Value = o.Id.ToString(),
                    Group = o.Status,
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "unitId", o.UnitId.ToString() },
                        { "unitNumber", o.Unit?.UnitNumber ?? "" },
                        { "ownerId", o.OwnerId.ToString() },
                        { "startDate", o.StartDate.ToString("yyyy-MM-dd") },
                        { "endDate", o.EndDate?.ToString("yyyy-MM-dd") ?? "" },
                        { "status", o.Status }
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
                _logger.LogError(ex, "Error occurred while retrieving ownership lookup data");
                return ApiResponse<LookupResponse<LookupDto>>.CreateError("Failed to retrieve ownership lookup data");
            }
        }
    }
} 