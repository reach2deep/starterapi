using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using starterkit.Core.Modules.Common;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services
{
    /// <summary>
    /// Service interface for managing societies
    /// </summary>
    public interface ISocietyService
    {
        /// <summary>
        /// Gets all societies
        /// </summary>
        Task<ApiResponse<IEnumerable<SocietyResponse>>> GetAllAsync();

        /// <summary>
        /// Gets a society by ID
        /// </summary>
        Task<ApiResponse<SocietyResponse>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a paged list of societies
        /// </summary>
        Task<ApiResponse<PagedResponse<SocietyResponse>>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Creates a new society
        /// </summary>
        Task<ApiResponse<SocietyResponse>> CreateAsync(CreateSocietyRequest request);

        /// <summary>
        /// Updates an existing society
        /// </summary>
        Task<ApiResponse<SocietyResponse>> UpdateAsync(UpdateSocietyRequest request);

        /// <summary>
        /// Deletes a society
        /// </summary>
        Task<ApiResponse<bool>> DeleteAsync(Guid id);

        /// <summary>
        /// Gets a society with all its related details
        /// </summary>
        Task<ApiResponse<SocietyResponse>> GetByIdWithDetailsAsync(Guid id);

        /// <summary>
        /// Gets societies data optimized for dropdown/lookup controls
        /// </summary>
        Task<ApiResponse<LookupResponse<LookupDto>>> GetLookupAsync(LookupRequest request);
    }
} 