using AutoMapper;
using Microsoft.Extensions.Logging;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Application.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Tenant.Interfaces.Repositories;

namespace starterkit.Application.Modules.Tenant.PermissionManagement.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            IPermissionRepository permissionRepository,
            IMapper mapper,
            ILogger<PermissionService> logger)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<PermissionDto>>> GetAllPermissionsAsync()
        {
            try
            {
                _logger.LogInformation("Getting all permissions");
                var permissions = await _permissionRepository.GetAllAsync();
                var permissionDtos = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                return ApiResponse<IEnumerable<PermissionDto>>.CreateSuccess(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all permissions");
                return ApiResponse<IEnumerable<PermissionDto>>.CreateError("Error retrieving permissions");
            }
        }

        public async Task<ApiResponse<PermissionDto>> GetPermissionByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Getting permission with ID: {Id}", id);
                var permission = await _permissionRepository.GetByIdAsync(id);
                if (permission == null)
                {
                    return ApiResponse<PermissionDto>.CreateError("Permission not found", "NOT_FOUND");
                }

                var permissionDto = _mapper.Map<PermissionDto>(permission);
                return ApiResponse<PermissionDto>.CreateSuccess(permissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission with ID: {Id}", id);
                return ApiResponse<PermissionDto>.CreateError("Error retrieving permission");
            }
        }

        public async Task<ApiResponse<IEnumerable<PermissionDto>>> GetPermissionsByModuleAsync(string module)
        {
            try
            {
                _logger.LogInformation("Getting permissions for module: {Module}", module);
                var permissions = await _permissionRepository.GetByModuleAsync(module);
                var permissionDtos = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                return ApiResponse<IEnumerable<PermissionDto>>.CreateSuccess(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for module: {Module}", module);
                return ApiResponse<IEnumerable<PermissionDto>>.CreateError("Error retrieving permissions");
            }
        }

        public async Task<ApiResponse<IEnumerable<PermissionDto>>> GetPermissionsByRoleAsync(Guid roleId)
        {
            try
            {
                _logger.LogInformation("Getting permissions for role ID: {RoleId}", roleId);
                var permissions = await _permissionRepository.GetByRoleIdAsync(roleId);
                var permissionDtos = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                return ApiResponse<IEnumerable<PermissionDto>>.CreateSuccess(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for role ID: {RoleId}", roleId);
                return ApiResponse<IEnumerable<PermissionDto>>.CreateError("Error retrieving permissions");
            }
        }

        public async Task<ApiResponse<PermissionDto>> CreatePermissionAsync(CreatePermissionRequest request)
        {
            try
            {
                _logger.LogInformation("Creating new permission: {Name}", request.Name);
                var permission = _mapper.Map<Permission>(request);
                var createdPermission = await _permissionRepository.AddAsync(permission);
                var permissionDto = _mapper.Map<PermissionDto>(createdPermission);
                return ApiResponse<PermissionDto>.CreateSuccess(permissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission: {Name}", request.Name);
                return ApiResponse<PermissionDto>.CreateError("Error creating permission");
            }
        }

        public async Task<ApiResponse<PermissionDto>> UpdatePermissionAsync(Guid id, UpdatePermissionRequest request)
        {
            try
            {
                _logger.LogInformation("Updating permission with ID: {Id}", id);
                
                var existingPermission = await _permissionRepository.GetByIdAsync(id);
                if (existingPermission == null)
                {
                    return ApiResponse<PermissionDto>.CreateError("Permission not found", "NOT_FOUND");
                }

                _mapper.Map(request, existingPermission);
                var updatedPermission = await _permissionRepository.UpdateAsync(existingPermission);
                var permissionDto = _mapper.Map<PermissionDto>(updatedPermission);
                return ApiResponse<PermissionDto>.CreateSuccess(permissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating permission with ID: {Id}", id);
                return ApiResponse<PermissionDto>.CreateError("Error updating permission");
            }
        }

        public async Task<ApiResponse<bool>> DeletePermissionAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting permission with ID: {Id}", id);
                
                if (!await _permissionRepository.ExistsAsync(id))
                {
                    return ApiResponse<bool>.CreateError("Permission not found", "NOT_FOUND");
                }

                await _permissionRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting permission with ID: {Id}", id);
                return ApiResponse<bool>.CreateError("Error deleting permission");
            }
        }
    }
} 