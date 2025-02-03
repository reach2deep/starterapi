using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.API.Controllers;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.RoleManagement.Interfaces;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.RoleManagement
{
    [Route("api/v1/tenant/[controller]")]
    [Authorize]
    public class RoleController : BaseApiController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Gets a role by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all roles
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _roleService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a paged list of roles
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<RoleResponse>>), 200)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _roleService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new role
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            var result = await _roleService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing role
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequest request)
        {
            var result = await _roleService.UpdateAsync(id, request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a role
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _roleService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets roles for a user
        /// </summary>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleResponse>>), 200)]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            var result = await _roleService.GetByUserIdAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Assigns permissions to a role
        /// </summary>
        [HttpPost("{roleId}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> AssignPermissions(Guid roleId, [FromBody] AssignPermissionsRequest request)
        {
            var result = await _roleService.AssignPermissionsAsync(roleId, request);
            return Ok(result);
        }

        /// <summary>
        /// Removes permissions from a role
        /// </summary>
        [HttpDelete("{roleId}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> RemovePermissions(Guid roleId, [FromBody] RemovePermissionsRequest request)
        {
            var result = await _roleService.RemovePermissionsAsync(roleId, request);
            return Ok(result);
        }

        /// <summary>
        /// Gets all permissions for a role
        /// </summary>
        [HttpGet("{roleId}/permissions")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionResponse>>), 200)]
        public async Task<IActionResult> GetPermissions(Guid roleId)
        {
            var result = await _roleService.GetPermissionsAsync(roleId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a copy of an existing role
        /// </summary>
        [HttpPost("{sourceRoleId}/copy")]
        [ProducesResponseType(typeof(ApiResponse<RoleResponse>), 200)]
        public async Task<IActionResult> CopyRole(Guid sourceRoleId, [FromBody] CopyRoleRequest request)
        {
            var result = await _roleService.CopyRoleAsync(sourceRoleId, request);
            return Ok(result);
        }

        /// <summary>
        /// Assigns roles to a user
        /// </summary>
        [HttpPost("user/{userId}/assign")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> AssignRolesToUser(Guid userId, [FromBody] AssignUserRolesRequest request)
        {
            var result = await _roleService.AssignRolesToUserAsync(userId, request);
            return Ok(result);
        }

        /// <summary>
        /// Removes roles from a user
        /// </summary>
        [HttpDelete("user/{userId}/remove")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> RemoveRolesFromUser(Guid userId, [FromBody] RemoveUserRolesRequest request)
        {
            var result = await _roleService.RemoveRolesFromUserAsync(userId, request);
            return Ok(result);
        }
    }
} 