using starterkit.starterkit.Application.Modules.Global.TenantManagement.DTOs;
using starterkit.starterkit.Core.Modules.Common;

namespace starterkit.starterkit.Application.Modules.Global.TenantManagement.Interfaces
{
    public interface ITenantService
    {
        Task<TenantResponseDto> CreateTenantAsync(CreateTenantRequestDto request);
        Task<TenantResponseDto> UpdateTenantAsync(Guid id, UpdateTenantRequestDto request);
        Task<bool> DeactivateTenantAsync(Guid id);
        Task<PagedResponse<TenantResponseDto>> GetTenantsAsync(int pageNumber = 1, int pageSize = 10);
        Task<TenantResponseDto> GetTenantByIdAsync(Guid id);
    }
} 