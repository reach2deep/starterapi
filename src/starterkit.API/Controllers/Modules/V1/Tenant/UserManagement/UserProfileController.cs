using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Application.Modules.Tenant.RoleManagement.Interfaces;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Core.Modules.Tenant;

namespace starterkit.API.Controllers.Modules.V1.Tenant.UserManagement
{
    /// <summary>
    /// Controller for managing user profiles in tenant context
    /// </summary>
    [Route("api/v1/tenant/profiles")]
    [Authorize]
    public class UserProfileController : BaseApiController
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IRoleService _roleService;

        public UserProfileController(
            IUserProfileService userProfileService,
            IRoleService roleService)
        {
            _userProfileService = userProfileService;
            _roleService = roleService;
        }

        /// <summary>
        /// Get the profile of the currently authenticated user
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized<UserProfileResponseDto>();
            }

            var result = await _userProfileService.GetMyProfileAsync(userId.Value);
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Update the profile of the currently authenticated user
        /// </summary>
        [HttpPut("me")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> UpdateMyProfile(
            [FromBody] UpdateUserProfileRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized<UserProfileResponseDto>();
            }

            var result = await _userProfileService.UpdateProfileAsync(userId.Value, request);
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Get a user profile by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> GetProfile(Guid id)
        {
            var result = await _userProfileService.GetProfileAsync(id);
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Get roles and permissions of the currently authenticated user for RBAC
        /// </summary>
        /// <returns>User's roles and unique permissions</returns>
        /// <response code="200">Returns the user's roles and unique permissions</response>
        /// <response code="401">If user is not authenticated</response>
        /// <response code="400">If there was an error retrieving roles</response>
        [HttpGet("mypermissions")]
        public async Task<ActionResult<ApiResponse<UserPermissionsResponse>>> GetMyPermissions()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized<UserPermissionsResponse>();
            }

            // Get user's roles
            var rolesResult = await _roleService.GetByUserIdAsync(userId.Value);
            if (!rolesResult.Success)
            {
                return BadRequest(ApiResponse<UserPermissionsResponse>.CreateError(
                    "Failed to retrieve user roles",  // Generic error message
                    "ROLES_RETRIEVAL_ERROR"          // Error code
                ));
            }

            // Get unique permissions from all roles
            // Using HashSet with custom equality comparer to ensure uniqueness based on Module and Action
            var uniquePermissions = new HashSet<PermissionResponse>(new PermissionEqualityComparer());
            
            foreach (var role in rolesResult.Data)
            {
                var rolePermissions = await _roleService.GetPermissionsAsync(role.Id);
                if (rolePermissions.Success && rolePermissions.Data != null)
                {
                    foreach (var permission in rolePermissions.Data)
                    {
                        uniquePermissions.Add(permission);
                    }
                }
            }

            var response = new UserPermissionsResponse
            {
                Roles = rolesResult.Data.ToList(),
                UniquePermissions = uniquePermissions.ToList()
            };

            return Ok(ApiResponse<UserPermissionsResponse>.CreateSuccess(response));
        }
    }

    /// <summary>
    /// Comparer for ensuring permission uniqueness based on Module and Action
    /// </summary>
    internal class PermissionEqualityComparer : IEqualityComparer<PermissionResponse>
    {
        public bool Equals(PermissionResponse x, PermissionResponse y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.Module == y.Module && x.Action == y.Action;
        }

        public int GetHashCode(PermissionResponse obj)
        {
            if (obj is null) return 0;
            var hash = 17;
            hash = hash * 23 + (obj.Module?.GetHashCode() ?? 0);
            hash = hash * 23 + (obj.Action?.GetHashCode() ?? 0);
            return hash;
        }
    }
}