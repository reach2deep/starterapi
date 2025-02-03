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
using System.Linq;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Services
{
    /// <summary>
    /// Implementation of IUnitService
    /// </summary>
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UnitService> _logger;
        private readonly IValidator<CreateUnitRequest> _createValidator;
        private readonly IValidator<UpdateUnitRequest> _updateValidator;

        public UnitService(
            IUnitRepository unitRepository,
            IFloorRepository floorRepository,
            IMapper mapper,
            ILogger<UnitService> logger,
            IValidator<CreateUnitRequest> createValidator,
            IValidator<UpdateUnitRequest> updateValidator)
        {
            _unitRepository = unitRepository ?? throw new ArgumentNullException(nameof(unitRepository));
            _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResponse>>> GetAllAsync()
        {
            try
            {
                var units = await _unitRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<UnitResponse>>(units);
                return ApiResponse<IEnumerable<UnitResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all units");
                return ApiResponse<IEnumerable<UnitResponse>>.CreateError("Failed to retrieve units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<UnitResponse>>> GetByFloorIdAsync(Guid floorId)
        {
            try
            {
                if (!await _floorRepository.ExistsAsync(floorId))
                    return ApiResponse<IEnumerable<UnitResponse>>.CreateError("Floor not found");

                var units = await _unitRepository.GetByFloorIdAsync(floorId);
                var response = _mapper.Map<IEnumerable<UnitResponse>>(units);
                return ApiResponse<IEnumerable<UnitResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving units for floor ID: {FloorId}", floorId);
                return ApiResponse<IEnumerable<UnitResponse>>.CreateError("Failed to retrieve units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var unit = await _unitRepository.GetByIdAsync(id);
                if (unit == null)
                    return ApiResponse<UnitResponse>.CreateError("Unit not found");

                var response = _mapper.Map<UnitResponse>(unit);
                return ApiResponse<UnitResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving unit with ID: {Id}", id);
                return ApiResponse<UnitResponse>.CreateError("Failed to retrieve unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResponse>> CreateAsync(CreateUnitRequest request)
        {
            try
            {
                // Validate request
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UnitResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // Check if floor exists
                if (!await _floorRepository.ExistsAsync(request.FloorId))
                    return ApiResponse<UnitResponse>.CreateError("Floor not found");

                // Map and create
                var unit = _mapper.Map<Unit>(request);
                var createdUnit = await _unitRepository.AddAsync(unit);
                var response = _mapper.Map<UnitResponse>(createdUnit);
                return ApiResponse<UnitResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating unit: {UnitNumber}", request.UnitNumber);
                return ApiResponse<UnitResponse>.CreateError("Failed to create unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<UnitResponse>> UpdateAsync(UpdateUnitRequest request)
        {
            try
            {
                // Validate request
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UnitResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // Check if unit exists
                var existingUnit = await _unitRepository.GetByIdAsync(request.Id);
                if (existingUnit == null)
                    return ApiResponse<UnitResponse>.CreateError("Unit not found");

                // Check if floor exists
                if (!await _floorRepository.ExistsAsync(request.FloorId))
                    return ApiResponse<UnitResponse>.CreateError("Floor not found");

                // Map and update
                _mapper.Map(request, existingUnit);
                var updatedUnit = await _unitRepository.UpdateAsync(existingUnit);
                var response = _mapper.Map<UnitResponse>(updatedUnit);
                return ApiResponse<UnitResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating unit with ID: {Id}", request.Id);
                return ApiResponse<UnitResponse>.CreateError("Failed to update unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _unitRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Unit not found");

                var result = await _unitRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting unit with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete unit");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<PagedResponse<UnitResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (units, totalCount) = await _unitRepository.GetPagedAsync(pageNumber, pageSize);
                var mappedUnits = _mapper.Map<IEnumerable<UnitResponse>>(units);
                var response = new PagedResponse<UnitResponse>(mappedUnits, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<UnitResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged units");
                return ApiResponse<PagedResponse<UnitResponse>>.CreateError("Failed to retrieve paged units");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request)
        {
            try
            {
                // Get all units
                var units = await _unitRepository.GetAllAsync();
                
                // Start with base query
                var filteredUnits = units.AsQueryable();

                // Apply active filter unless specifically requested to include inactive
                if (!request.IncludeInactive)
                {
                    filteredUnits = filteredUnits.Where(u => u.IsActive);
                }

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredUnits = filteredUnits.Where(u => 
                        u.UnitNumber.ToLower().Contains(searchTerm));
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
                                    filteredUnits = filteredUnits.Where(u => u.Id == unitId);
                                }
                                break;
                            case "floornumber":
                                if (int.TryParse(filter.Value, out var floorNumber))
                                {
                                    filteredUnits = filteredUnits.Where(u => u.Floor.FloorNumber == floorNumber);
                                }
                                break;
                        }
                    }
                }

                // Apply sorting if requested
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var isAscending = string.IsNullOrWhiteSpace(request.SortDirection) || 
                                    request.SortDirection.ToLower() == "asc";

                    filteredUnits = request.SortBy.ToLower() switch
                    {
                        "unitnumber" => isAscending 
                            ? filteredUnits.OrderBy(u => u.UnitNumber)
                            : filteredUnits.OrderByDescending(u => u.UnitNumber),
                        "floornumber" => isAscending 
                            ? filteredUnits.OrderBy(u => u.Floor.FloorNumber)
                            : filteredUnits.OrderByDescending(u => u.Floor.FloorNumber),
                        _ => filteredUnits.OrderBy(u => u.UnitNumber) // Default sort by unit number
                    };
                }

                // Convert to list for pagination
                var unitsList = filteredUnits.ToList();
                var totalCount = unitsList.Count;

                // Apply pagination if requested
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    var pageNumber = Math.Max(1, request.Page.Value);
                    var pageSize = Math.Max(1, request.PageSize.Value);
                    unitsList = unitsList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Map to lookup DTOs
                var lookupItems = unitsList.Select(u => new LookupDto
                {
                    Id = u.Id,
                    Label = $"Unit {u.UnitNumber}",
                    Value = u.Id.ToString(),
                    Group = $"{u.Floor?.Block?.Name} - Floor {u.Floor?.FloorNumber}", // Group by block and floor
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "floorId", u.FloorId.ToString() },
                        { "floorNumber", u.Floor?.FloorNumber.ToString() ?? "" },
                        { "blockId", u.Floor?.BlockId.ToString() ?? "" },
                        { "blockName", u.Floor?.Block?.Name ?? "" },
                        { "unitNumber", u.UnitNumber }
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
                _logger.LogError(ex, "Error occurred while retrieving unit lookup data");
                return ApiResponse<LookupResponse<LookupDto>>.CreateError("Failed to retrieve unit lookup data");
            }
        }
    }
} 