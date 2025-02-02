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
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<RoleResponse>.CreateError("Role not found", "NOT_FOUND");

            var response = _mapper.Map<RoleResponse>(role);
            return ApiResponse<RoleResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<IEnumerable<RoleResponse>>> GetAllAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<RoleResponse>>(roles);
            return ApiResponse<IEnumerable<RoleResponse>>.CreateSuccess(response);
        }

        public async Task<ApiResponse<PagedResponse<RoleResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var (roles, totalCount) = await _roleRepository.GetPagedAsync(pageNumber, pageSize);
            var mappedRoles = _mapper.Map<IEnumerable<RoleResponse>>(roles);
            var response = new PagedResponse<RoleResponse>(mappedRoles, totalCount, pageNumber, pageSize);
            return ApiResponse<PagedResponse<RoleResponse>>.CreateSuccess(response);
        }

        public async Task<ApiResponse<RoleResponse>> CreateAsync(CreateRoleRequest request)
        {
            if (await _roleRepository.ExistsByNameAsync(request.Name))
                return ApiResponse<RoleResponse>.CreateError($"Role with name '{request.Name}' already exists", "DUPLICATE_NAME");

            var role = _mapper.Map<Role>(request);
            role = await _roleRepository.CreateAsync(role);
            var response = _mapper.Map<RoleResponse>(role);
            return ApiResponse<RoleResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<RoleResponse>> UpdateAsync(Guid id, UpdateRoleRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<RoleResponse>.CreateError("Role not found", "NOT_FOUND");

            if (role.Name != request.Name && await _roleRepository.ExistsByNameAsync(request.Name))
                return ApiResponse<RoleResponse>.CreateError($"Role with name '{request.Name}' already exists", "DUPLICATE_NAME");

            _mapper.Map(request, role);
            role = await _roleRepository.UpdateAsync(role);
            var response = _mapper.Map<RoleResponse>(role);
            return ApiResponse<RoleResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                return ApiResponse<bool>.CreateError("Role not found", "NOT_FOUND");

            await _roleRepository.DeleteAsync(id);
            return ApiResponse<bool>.CreateSuccess(true);
        }

        public async Task<ApiResponse<IEnumerable<RoleResponse>>> GetByUserIdAsync(Guid userId)
        {
            var roles = await _roleRepository.GetByUserIdAsync(userId);
            var response = _mapper.Map<IEnumerable<RoleResponse>>(roles);
            return ApiResponse<IEnumerable<RoleResponse>>.CreateSuccess(response);
        }

        public async Task<ApiResponse<bool>> AssignPermissionsAsync(Guid roleId, AssignPermissionsRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.CreateError("Role not found", "NOT_FOUND");

            await _roleRepository.AssignPermissionsAsync(roleId, request.PermissionIds);
            return ApiResponse<bool>.CreateSuccess(true);
        }

        public async Task<ApiResponse<bool>> RemovePermissionsAsync(Guid roleId, RemovePermissionsRequest request)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<bool>.CreateError("Role not found", "NOT_FOUND");

            await _roleRepository.RemovePermissionsAsync(roleId, request.PermissionIds);
            return ApiResponse<bool>.CreateSuccess(true);
        }

        public async Task<ApiResponse<IEnumerable<PermissionResponse>>> GetPermissionsAsync(Guid roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                return ApiResponse<IEnumerable<PermissionResponse>>.CreateError("Role not found", "NOT_FOUND");

            var permissions = await _roleRepository.GetPermissionsAsync(roleId);
            var response = _mapper.Map<IEnumerable<PermissionResponse>>(permissions);
            return ApiResponse<IEnumerable<PermissionResponse>>.CreateSuccess(response);
        }

        public async Task<ApiResponse<RoleResponse>> CopyRoleAsync(Guid sourceRoleId, CopyRoleRequest request)
        {
            var sourceRole = await _roleRepository.GetByIdAsync(sourceRoleId);
            if (sourceRole == null)
                return ApiResponse<RoleResponse>.CreateError("Source role not found", "NOT_FOUND");

            if (await _roleRepository.ExistsByNameAsync(request.NewName))
                return ApiResponse<RoleResponse>.CreateError($"Role with name '{request.NewName}' already exists", "DUPLICATE_NAME");

            // Create new role
            var newRole = new Role
            {
                Name = request.NewName,
                Description = request.Description,
                IsDefault = false
            };

            // Copy role
            newRole = await _roleRepository.CreateAsync(newRole);

            // Copy permissions
            var permissions = await _roleRepository.GetPermissionsAsync(sourceRoleId);
            if (permissions.Any())
            {
                await _roleRepository.AssignPermissionsAsync(newRole.Id, permissions.Select(p => p.Id));
            }

            var response = _mapper.Map<RoleResponse>(newRole);
            return ApiResponse<RoleResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<bool>> AssignRolesToUserAsync(Guid userId, AssignUserRolesRequest request)
        {
            // Validate user exists
            var userRoles = await _roleRepository.GetByUserIdAsync(userId);
            var existingRoleIds = userRoles.Select(r => r.Id).ToList();

            // Get new roles to assign (exclude existing ones)
            var newRoleIds = request.RoleIds.Except(existingRoleIds).ToList();

            if (!newRoleIds.Any())
                return ApiResponse<bool>.CreateSuccess(true); // Nothing to assign

            // Validate all roles exist
            foreach (var roleId in newRoleIds)
            {
                var role = await _roleRepository.GetByIdAsync(roleId);
                if (role == null)
                    return ApiResponse<bool>.CreateError($"Role with ID {roleId} not found", "NOT_FOUND");
            }

            // Create UserRole entities
            var userRolesToAdd = newRoleIds.Select(roleId => new UserRole
            {
                UserId = userId,
                RoleId = roleId
            });

            // Add user roles
            foreach (var userRole in userRolesToAdd)
            {
                await _roleRepository.CreateUserRoleAsync(userRole);
            }

            return ApiResponse<bool>.CreateSuccess(true);
        }

        public async Task<ApiResponse<bool>> RemoveRolesFromUserAsync(Guid userId, RemoveUserRolesRequest request)
        {
            // Validate user exists
            var userRoles = await _roleRepository.GetByUserIdAsync(userId);
            if (!userRoles.Any())
                return ApiResponse<bool>.CreateError($"No roles found for user with ID {userId}", "NOT_FOUND");

            // Remove roles
            await _roleRepository.RemoveUserRolesAsync(userId, request.RoleIds);

            return ApiResponse<bool>.CreateSuccess(true);
        }
    }
} 