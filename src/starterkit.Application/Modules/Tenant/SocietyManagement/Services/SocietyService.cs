using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
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
    /// Implementation of ISocietyService
    /// </summary>
    public class SocietyService : ISocietyService
    {
        private readonly ISocietyRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<SocietyService> _logger;
        private readonly IValidator<CreateSocietyRequest> _createValidator;
        private readonly IValidator<UpdateSocietyRequest> _updateValidator;

        public SocietyService(
            ISocietyRepository repository,
            IMapper mapper,
            ILogger<SocietyService> logger,
            IValidator<CreateSocietyRequest> createValidator,
            IValidator<UpdateSocietyRequest> updateValidator)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<IEnumerable<SocietyResponse>>> GetAllAsync()
        {
            try
            {
                var societies = await _repository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<SocietyResponse>>(societies);
                return ApiResponse<IEnumerable<SocietyResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all societies");
                return ApiResponse<IEnumerable<SocietyResponse>>.CreateError("Failed to retrieve societies");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<SocietyResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var society = await _repository.GetByIdAsync(id);
                if (society == null)
                    return ApiResponse<SocietyResponse>.CreateError("Society not found");

                var response = _mapper.Map<SocietyResponse>(society);
                return ApiResponse<SocietyResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society with ID: {Id}", id);
                return ApiResponse<SocietyResponse>.CreateError("Failed to retrieve society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<SocietyResponse>> CreateAsync(CreateSocietyRequest request)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<SocietyResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                // if (await _repository.ExistsByNameAsync(request.Name))
                //     return ApiResponse<SocietyResponse>.CreateError("Society with this name already exists");

                if (await _repository.ExistsByRegistrationNumberAsync(request.RegistrationNumber))
                    return ApiResponse<SocietyResponse>.CreateError("Society with this registration number already exists");

                var society = _mapper.Map<Society>(request);
                var createdSociety = await _repository.AddAsync(society);
                var response = _mapper.Map<SocietyResponse>(createdSociety);
                return ApiResponse<SocietyResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating society: {Name}", request.Name);
                return ApiResponse<SocietyResponse>.CreateError("Failed to create society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<SocietyResponse>> UpdateAsync(UpdateSocietyRequest request)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<SocietyResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

                var existingSociety = await _repository.GetByIdAsync(request.Id);
                if (existingSociety == null)
                    return ApiResponse<SocietyResponse>.CreateError("Society not found");

                // if (await _repository.ExistsByNameAsync(request.Name, request.Id))
                //     return ApiResponse<SocietyResponse>.CreateError("Society with this name already exists");

                // if (await _repository.ExistsByRegistrationNumberAsync(request.RegistrationNumber, request.Id))
                //     return ApiResponse<SocietyResponse>.CreateError("Society with this registration number already exists");

                _mapper.Map(request, existingSociety);
                var updatedSociety = await _repository.UpdateAsync(existingSociety);
                var response = _mapper.Map<SocietyResponse>(updatedSociety);
                return ApiResponse<SocietyResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating society with ID: {Id}", request.Id);
                return ApiResponse<SocietyResponse>.CreateError("Failed to update society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                if (!await _repository.ExistsAsync(id))
                    return ApiResponse<bool>.CreateError("Society not found");

                var result = await _repository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting society with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Failed to delete society");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<SocietyResponse>> GetByIdWithDetailsAsync(Guid id)
        {
            try
            {
                var society = await _repository.GetByIdWithDetailsAsync(id);
                if (society == null)
                    return ApiResponse<SocietyResponse>.CreateError("Society not found");

                var response = _mapper.Map<SocietyResponse>(society);
                return ApiResponse<SocietyResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving society details with ID: {Id}", id);
                return ApiResponse<SocietyResponse>.CreateError("Failed to retrieve society details");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<PagedResponse<SocietyResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (societies, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize);
                var mappedSocieties = _mapper.Map<IEnumerable<SocietyResponse>>(societies);
                var response = new PagedResponse<SocietyResponse>(mappedSocieties, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<SocietyResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving paged societies");
                return ApiResponse<PagedResponse<SocietyResponse>>.CreateError("Failed to retrieve paged societies");
            }
        }

        /// <inheritdoc/>
        public async Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request)
        {
            try
            {
                // Get all societies
                var societies = await _repository.GetAllAsync();
                
                // Start with base query
                var filteredSocieties = societies.AsQueryable();

                // Apply active filter unless specifically requested to include inactive
                if (!request.IncludeInactive)
                {
                    filteredSocieties = filteredSocieties.Where(s => s.IsActive);
                }

                // Apply search if provided
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    filteredSocieties = filteredSocieties
                        .Where(s => s.Name.ToLower().Contains(searchTerm) || 
                                  s.RegistrationNumber.ToLower().Contains(searchTerm));
                }

                // Apply optional filters if provided
                if (request.Filters?.Any() == true)
                {
                    foreach (var filter in request.Filters)
                    {
                        // Skip empty filters
                        if (string.IsNullOrWhiteSpace(filter.Value)) continue;

                        switch (filter.Key.ToLower())
                        {
                            case "registrationnumber":
                                filteredSocieties = filteredSocieties.Where(s => 
                                    s.RegistrationNumber.ToLower().Contains(filter.Value.ToLower()));
                                break;
                            case "city":
                                filteredSocieties = filteredSocieties.Where(s => 
                                    s.Address != null && s.Address.City.ToLower().Contains(filter.Value.ToLower()));
                                break;
                            case "state":
                                filteredSocieties = filteredSocieties.Where(s => 
                                    s.Address != null && s.Address.State.ToLower().Contains(filter.Value.ToLower()));
                                break;
                        }
                    }
                }

                // Apply sorting if requested
                if (!string.IsNullOrWhiteSpace(request.SortBy))
                {
                    var isAscending = string.IsNullOrWhiteSpace(request.SortDirection) || 
                                    request.SortDirection.ToLower() == "asc";

                    filteredSocieties = request.SortBy.ToLower() switch
                    {
                        "name" => isAscending 
                            ? filteredSocieties.OrderBy(s => s.Name)
                            : filteredSocieties.OrderByDescending(s => s.Name),
                        "registrationnumber" => isAscending 
                            ? filteredSocieties.OrderBy(s => s.RegistrationNumber)
                            : filteredSocieties.OrderByDescending(s => s.RegistrationNumber),
                        "city" => isAscending 
                            ? filteredSocieties.OrderBy(s => s.Address.City)
                            : filteredSocieties.OrderByDescending(s => s.Address.City),
                        _ => filteredSocieties.OrderBy(s => s.Name) // Default sort by name
                    };
                }

                // Convert to list for pagination
                var societiesList = filteredSocieties.ToList();
                var totalCount = societiesList.Count;

                // Apply pagination if requested
                if (request.Page.HasValue && request.PageSize.HasValue)
                {
                    var pageNumber = Math.Max(1, request.Page.Value);
                    var pageSize = Math.Max(1, request.PageSize.Value);
                    societiesList = societiesList
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                }

                // Map to lookup DTOs
                var lookupItems = societiesList.Select(s => new LookupDto
                {
                    Id = s.Id,
                    Label = s.Name,
                    Value = s.Id.ToString(),
                    Group = s.Address?.City, // Optional grouping by city
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "registrationNumber", s.RegistrationNumber },
                        { "city", s.Address?.City ?? "" },
                        { "state", s.Address?.State ?? "" }
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
                _logger.LogError(ex, "Error occurred while retrieving society lookup data");
                return ApiResponse<LookupResponse<LookupDto>>.CreateError("Failed to retrieve society lookup data");
            }
        }
    }
} 