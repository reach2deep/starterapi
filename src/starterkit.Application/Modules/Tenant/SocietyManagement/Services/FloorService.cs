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
    /// Implementation of IFloorService
    /// </summary>
    public class FloorService : IFloorService
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<FloorService> _logger;
        private readonly IValidator<CreateFloorRequest> _createValidator;
        private readonly IValidator<UpdateFloorRequest> _updateValidator;

        /// <summary>
        /// Initializes a new instance of the FloorService class.
        /// </summary>
        /// <param name="floorRepository">The floor repository.</param>
        /// <param name="blockRepository">The block repository.</param>
        /// <param name="mapper">The mapper instance.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="createValidator">The create validator instance.</param>
        /// <param name="updateValidator">The update validator instance.</param>
        public FloorService(
            IFloorRepository floorRepository,
            IBlockRepository blockRepository,
            IMapper mapper,
            ILogger<FloorService> logger,
            IValidator<CreateFloorRequest> createValidator,
            IValidator<UpdateFloorRequest> updateValidator)
        {
            _floorRepository = floorRepository ?? throw new ArgumentNullException(nameof(floorRepository));
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<FloorResponse>>> GetAllAsync()
        {
            try
            {
                var floors = await _floorRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<FloorResponse>>(floors);
                return ApiResponse<IEnumerable<FloorResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all floors");
                return ApiResponse<IEnumerable<FloorResponse>>.CreateError("Failed to retrieve floors");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<FloorResponse>>> GetByBlockIdAsync(Guid blockId)
        {
            try
            {
                if (!await _blockRepository.ExistsAsync(blockId))
                    return ApiResponse<IEnumerable<FloorResponse>>.CreateError("Block not found");

                var floors = await _floorRepository.GetByBlockIdAsync(blockId);
                var response = _mapper.Map<IEnumerable<FloorResponse>>(floors);
                return ApiResponse<IEnumerable<FloorResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floors for block ID: {BlockId}", blockId);
                return ApiResponse<IEnumerable<FloorResponse>>.CreateError("Failed to retrieve floors");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<FloorResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var floor = await _floorRepository.GetByIdAsync(id);
                if (floor == null)
                    return ApiResponse<FloorResponse>.CreateError("Floor not found");

                var response = _mapper.Map<FloorResponse>(floor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving floor with ID: {Id}", id);
                return ApiResponse<FloorResponse>.CreateError("Failed to retrieve floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<FloorResponse>> CreateAsync(CreateFloorRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<FloorResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                if (!await _blockRepository.ExistsAsync(request.BlockId))
                    return ApiResponse<FloorResponse>.CreateError("Block not found");

                if (!await _floorRepository.IsFloorNumberUniqueInBlockAsync(request.BlockId, request.FloorNumber))
                    return ApiResponse<FloorResponse>.CreateError("Floor number must be unique within the block");

                // 3. Map to entity
                var floor = _mapper.Map<Floor>(request);

                // 4. Save to database
                var createdFloor = await _floorRepository.AddAsync(floor);

                // 5. Return response
                var response = _mapper.Map<FloorResponse>(createdFloor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating floor: {Name}", request.Name);
                return ApiResponse<FloorResponse>.CreateError("Failed to create floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<FloorResponse>> UpdateAsync(UpdateFloorRequest request)
        {
            try
            {
                // 1. Validate request
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<FloorResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // 2. Business rules
                var existingFloor = await _floorRepository.GetByIdAsync(request.Id);
                if (existingFloor == null)
                    return ApiResponse<FloorResponse>.CreateError("Floor not found");

                if (!await _blockRepository.ExistsAsync(request.BlockId))
                    return ApiResponse<FloorResponse>.CreateError("Block not found");

                if (!await _floorRepository.IsFloorNumberUniqueInBlockAsync(request.BlockId, request.FloorNumber, request.Id))
                    return ApiResponse<FloorResponse>.CreateError("Floor number must be unique within the block");

                // 3. Map to entity
                _mapper.Map(request, existingFloor);

                // 4. Save to database
                var updatedFloor = await _floorRepository.UpdateAsync(existingFloor);

                // 5. Return response
                var response = _mapper.Map<FloorResponse>(updatedFloor);
                return ApiResponse<FloorResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating floor with ID: {Id}", request.Id);
                return ApiResponse<FloorResponse>.CreateError("Failed to update floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _floorRepository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Floor not found");

                var result = await _floorRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting floor with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete floor");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<PagedResponse<FloorResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (floors, totalCount) = await _floorRepository.GetPagedAsync(pageNumber, pageSize);
                var mappedFloors = _mapper.Map<IEnumerable<FloorResponse>>(floors);
                var response = new PagedResponse<FloorResponse>(mappedFloors, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<FloorResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged floor records");
                return ApiResponse<PagedResponse<FloorResponse>>.CreateError("Failed to retrieve paged floor records");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request)
        {
            try
            {
                // Get all floor records
                var floors = await _floorRepository.GetAllAsync();
                
                // Start with base query
                var filteredFloors = floors.AsQueryable();

                // Apply active filter unless specifically requested to include inactive
                if (!request.IncludeInactive)
                {
                    filteredFloors = filteredFloors.Where(f => f.IsActive);
                }

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredFloors = filteredFloors.Where(f => 
                        f.Name.ToLower().Contains(searchTerm) ||
                        f.FloorNumber.ToString().Contains(searchTerm) ||
                        (f.Block != null && f.Block.Name.ToLower().Contains(searchTerm)));
                }

                // Apply optional filters if provided
                if (request.Filters?.Any() == true)
                {
                    foreach (var filter in request.Filters)
                    {
                        if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                        switch (filter.Key.ToLower())
                        {
                            case "blockid":
                                if (Guid.TryParse(filter.Value, out var blockId))
                                {
                                    filteredFloors = filteredFloors.Where(f => f.BlockId == blockId);
                                }
                                break;
                            case "floornumber":
                                if (int.TryParse(filter.Value, out var floorNumber))
                                {
                                    filteredFloors = filteredFloors.Where(f => f.FloorNumber == floorNumber);
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

                    filteredFloors = request.SortBy.ToLower() switch
                    {
                        "name" => isAscending 
                            ? filteredFloors.OrderBy(f => f.Name)
                            : filteredFloors.OrderByDescending(f => f.Name),
                        "floornumber" => isAscending 
                            ? filteredFloors.OrderBy(f => f.FloorNumber)
                            : filteredFloors.OrderByDescending(f => f.FloorNumber),
                        "blockname" => isAscending 
                            ? filteredFloors.OrderBy(f => f.Block.Name)
                            : filteredFloors.OrderByDescending(f => f.Block.Name),
                        _ => filteredFloors.OrderBy(f => f.FloorNumber) // Default sort by floor number
                    };
                }

                // Convert to list for pagination
                var floorsList = filteredFloors.ToList();
                var totalCount = floorsList.Count;

                // Apply pagination if requested
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    var pageNumber = Math.Max(1, request.Page.Value);
                    var pageSize = Math.Max(1, request.PageSize.Value);
                    floorsList = floorsList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Map to lookup DTOs
                var lookupItems = floorsList.Select(f => new LookupDto
                {
                    Id = f.Id,
                    Label = $"{f.Block?.Name} - Floor {f.FloorNumber} ({f.Name})",
                    Value = f.Id.ToString(),
                    Group = f.Block?.Name ?? "Unassigned",
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "blockId", f.BlockId.ToString() },
                        { "blockName", f.Block?.Name ?? "" },
                        { "floorNumber", f.FloorNumber.ToString() },
                        { "name", f.Name },
                        { "isActive", f.IsActive.ToString() }
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
                _logger.LogError(ex, "Error occurred while retrieving floor lookup data");
                return ApiResponse<LookupResponse<LookupDto>>.CreateError("Failed to retrieve floor lookup data");
            }
        }
    }
} 