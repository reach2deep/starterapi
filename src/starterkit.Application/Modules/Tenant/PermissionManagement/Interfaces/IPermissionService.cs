using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Core.Modules.Common;

namespace starterkit.Application.Modules.Tenant.PermissionManagement.Interfaces
{
    public interface IPermissionService
    {
        Task<ApiResponse<IEnumerable<PermissionDto>>> GetAllPermissionsAsync();
        Task<ApiResponse<PermissionDto>> GetPermissionByIdAsync(Guid id);
        Task<ApiResponse<IEnumerable<PermissionDto>>> GetPermissionsByModuleAsync(string module);
        Task<ApiResponse<IEnumerable<PermissionDto>>> GetPermissionsByRoleAsync(Guid roleId);
        Task<ApiResponse<PermissionDto>> CreatePermissionAsync(CreatePermissionRequest request);
        Task<ApiResponse<PermissionDto>> UpdatePermissionAsync(Guid id, UpdatePermissionRequest request);
        Task<ApiResponse<bool>> DeletePermissionAsync(Guid id);
    }
} 