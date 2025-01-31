using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Application.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.PermissionManagement
{
    /// <summary>
    /// Controller for managing permissions in tenant context
    /// </summary>
    [Route("api/v1/tenant/permissions")]
    [Authorize]
    public class PermissionController : BaseApiController
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(
            IPermissionService permissionService,
            ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Get all permissions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDto>>>> GetAllPermissions()
        {
            var result = await _permissionService.GetAllPermissionsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get a permission by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PermissionDto>>> GetPermissionById(Guid id)
        {
            var result = await _permissionService.GetPermissionByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get permissions by module
        /// </summary>
        [HttpGet("module/{module}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDto>>>> GetPermissionsByModule(string module)
        {
            var result = await _permissionService.GetPermissionsByModuleAsync(module);
            return Ok(result);
        }

        /// <summary>
        /// Get permissions by role
        /// </summary>
        [HttpGet("role/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionDto>>>> GetPermissionsByRole(Guid roleId)
        {
            var result = await _permissionService.GetPermissionsByRoleAsync(roleId);
            return Ok(result);
        }

        /// <summary>
        /// Create a new permission
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PermissionDto>>> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            var result = await _permissionService.CreatePermissionAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetPermissionById), new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Update an existing permission
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PermissionDto>>> UpdatePermission(Guid id, [FromBody] UpdatePermissionRequest request)
        {
            var result = await _permissionService.UpdatePermissionAsync(id, request);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Delete a permission
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePermission(Guid id)
        {
            var result = await _permissionService.DeletePermissionAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
} 