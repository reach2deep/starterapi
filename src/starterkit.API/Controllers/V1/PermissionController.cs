using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Application.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class PermissionController : ControllerBase
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

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await _permissionService.GetAllPermissionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissionById(Guid id)
        {
            var result = await _permissionService.GetPermissionByIdAsync(id);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("module/{module}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPermissionsByModule(string module)
        {
            var result = await _permissionService.GetPermissionsByModuleAsync(module);
            return Ok(result);
        }

        [HttpGet("role/{roleId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<PermissionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPermissionsByRole(Guid roleId)
        {
            var result = await _permissionService.GetPermissionsByRoleAsync(roleId);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            var result = await _permissionService.CreatePermissionAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return CreatedAtAction(nameof(GetPermissionById), new { id = result.Data.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PermissionDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionRequest request)
        {
            var result = await _permissionService.UpdatePermissionAsync(id, request);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePermission(Guid id)
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