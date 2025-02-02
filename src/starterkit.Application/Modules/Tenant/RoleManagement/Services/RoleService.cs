using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.RoleManagement.Interfaces;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;

namespace starterkit.Application.Modules.Tenant.RoleManagement.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;
        private readonly IValidator<CreateRoleRequest> _createValidator;
        private readonly IValidator<UpdateRoleRequest> _updateValidator;
        private readonly IValidator<CopyRoleRequest> _copyValidator;

        public RoleService(
            IRoleRepository roleRepository,
            IMapper mapper,
            ILogger<RoleService> logger,
            IValidator<CreateRoleRequest> createValidator,
            IValidator<UpdateRoleRequest> updateValidator,
            IValidator<CopyRoleRequest> copyValidator)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _copyValidator = copyValidator;
        }

        public async Task<ApiResponse<RoleResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResponse<RoleResponse>.CreateError("Role not found");
                }

                var response = _mapper.Map<RoleResponse>(role);
                return ApiResponse<RoleResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role by ID {Id}", id);
                return ApiResponse<RoleResponse>.CreateError("Error getting role");
            }
        }

        public async Task<ApiResponse<IEnumerable<RoleResponse>>> GetAllAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                var response = _mapper.Map<IEnumerable<RoleResponse>>(roles);
                return ApiResponse<IEnumerable<RoleResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all roles");
                return ApiResponse<IEnumerable<RoleResponse>>.CreateError("Error getting roles");
            }
        }

        public async Task<ApiResponse<PagedResponse<RoleResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (roles, totalCount) = await _roleRepository.GetPagedAsync(pageNumber, pageSize);
                var response = _mapper.Map<IEnumerable<RoleResponse>>(roles);
                var pagedResponse = new PagedResponse<RoleResponse>(response, totalCount, pageNumber, pageSize);
                return ApiResponse<PagedResponse<RoleResponse>>.CreateSuccess(pagedResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged roles");
                return ApiResponse<PagedResponse<RoleResponse>>.CreateError("Error getting paged roles");
            }
        }

        public async Task<ApiResponse<RoleResponse>> CreateAsync(CreateRoleRequest request)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ApiResponse<RoleResponse>.CreateError("Validation failed", details: validationResult.Errors);
                }

                var role = _mapper.Map<Role>(request);
                var createdRole = await _roleRepository.CreateAsync(role);

                if (request.PermissionIds.Any())
                {
                    await _roleRepository.AssignPermissionsAsync(createdRole.Id, request.PermissionIds);
                }

                var response = _mapper.Map<RoleResponse>(createdRole);
                return ApiResponse<RoleResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return ApiResponse<RoleResponse>.CreateError("Error creating role");
            }
        }

        public async Task<ApiResponse<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ApiResponse<RoleResponse>.CreateError("Validation failed", details: validationResult.Errors);
                }

                var existingRole = await _roleRepository.GetByIdAsync(id);
                if (existingRole == null)
                {
                    return ApiResponse<RoleResponse>.CreateError("Role not found");
                }

                _mapper.Map(request, existingRole);
                var updatedRole = await _roleRepository.UpdateAsync(existingRole);
                var response = _mapper.Map<RoleResponse>(updatedRole);
                return ApiResponse<RoleResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role {Id}", id);
                return ApiResponse<RoleResponse>.CreateError("Error updating role");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    return ApiResponse<bool>.CreateError("Role not found");
                }

                if (role.IsDefault)
                {
                    return ApiResponse<bool>.CreateError("Cannot delete default role");
                }

                await _roleRepository.DeleteAsync(id);
                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role {Id}", id);
                return ApiResponse<bool>.CreateError("Error deleting role");
            }
        }

        public async Task<ApiResponse<IEnumerable<RoleResponse>>> GetByUserIdAsync(Guid userId)
        {
            try
            {
                var roles = await _roleRepository.GetByUserIdAsync(userId);
                var response = _mapper.Map<IEnumerable<RoleResponse>>(roles);
                return ApiResponse<IEnumerable<RoleResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
                return ApiResponse<IEnumerable<RoleResponse>>.CreateError("Error getting user roles");
            }
        }

        public async Task<ApiResponse<bool>> AssignPermissionsAsync(Guid roleId, AssignPermissionsRequest request)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.CreateError("Role not found");
                }

                await _roleRepository.AssignPermissionsAsync(roleId, request.PermissionIds);
                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to role {RoleId}", roleId);
                return ApiResponse<bool>.CreateError("Error assigning permissions");
            }
        }

        public async Task<ApiResponse<bool>> RemovePermissionsAsync(Guid roleId, RemovePermissionsRequest request)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<bool>.CreateError("Role not found");
                }

                await _roleRepository.RemovePermissionsAsync(roleId, request.PermissionIds);
                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing permissions from role {RoleId}", roleId);
                return ApiResponse<bool>.CreateError("Error removing permissions");
            }
        }

        public async Task<ApiResponse<IEnumerable<PermissionResponse>>> GetPermissionsAsync(Guid roleId)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                {
                    return ApiResponse<IEnumerable<PermissionResponse>>.CreateError("Role not found");
                }

                var permissions = await _roleRepository.GetPermissionsAsync(roleId);
                var response = _mapper.Map<IEnumerable<PermissionResponse>>(permissions);
                return ApiResponse<IEnumerable<PermissionResponse>>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for role {RoleId}", roleId);
                return ApiResponse<IEnumerable<PermissionResponse>>.CreateError("Error getting role permissions");
            }
        }

        public async Task<ApiResponse<RoleResponse>> CopyRoleAsync(Guid sourceRoleId, CopyRoleRequest request)
        {
            try
            {
                var validationResult = await _copyValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return ApiResponse<RoleResponse>.CreateError("Validation failed", details: validationResult.Errors);
                }

                var sourceRole = await _roleRepository.GetByIdAsync(sourceRoleId);
                if (sourceRole == null)
                {
                    return ApiResponse<RoleResponse>.CreateError("Source role not found");
                }

                var newRole = new Role
                {
                    Name = request.NewName,
                    Description = request.Description ?? sourceRole.Description,
                    IsDefault = false
                };

                var createdRole = await _roleRepository.CreateAsync(newRole);

                if (request.CopyPermissions)
                {
                    var permissions = await _roleRepository.GetPermissionsAsync(sourceRoleId);
                    var permissionIds = permissions.Select(p => p.Id).ToList();
                    if (permissionIds.Any())
                    {
                        await _roleRepository.AssignPermissionsAsync(createdRole.Id, permissionIds);
                    }
                }

                var response = _mapper.Map<RoleResponse>(createdRole);
                return ApiResponse<RoleResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error copying role {SourceRoleId}", sourceRoleId);
                return ApiResponse<RoleResponse>.CreateError("Error copying role");
            }
        }
    }
} 