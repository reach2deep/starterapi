using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.DTOs;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.starterkit.Core.Modules.Common;

namespace starterkit.starterkit.API.Controllers.Modules.V1.Global.TenantManagement
{
    [Route("api/v1/global/tenants")]
    [Authorize(Roles = "RootAdmin")]
    public class TenantController : BaseApiController
    {
        private readonly ITenantService _tenantService;

        public TenantController(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        /// <summary>
        /// Creates a new tenant and initializes its database
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TenantResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<TenantResponseDto>>> CreateTenant([FromBody] CreateTenantRequestDto request)
        {
            var tenant = await _tenantService.CreateTenantAsync(request);
            return Ok(ApiResponse<TenantResponseDto>.CreateSuccess(tenant));
        }

        /// <summary>
        /// Updates an existing tenant
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TenantResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<TenantResponseDto>>> UpdateTenant(Guid id, [FromBody] UpdateTenantRequestDto request)
        {
            var tenant = await _tenantService.UpdateTenantAsync(id, request);
            return Ok(ApiResponse<TenantResponseDto>.CreateSuccess(tenant));
        }

        /// <summary>
        /// Deactivates a tenant
        /// </summary>
        [HttpPatch("{id}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<ActionResult<ApiResponse<bool>>> DeactivateTenant(Guid id)
        {
            var result = await _tenantService.DeactivateTenantAsync(id);
            return Ok(ApiResponse<bool>.CreateSuccess(result));
        }

        /// <summary>
        /// Gets all tenants with pagination
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<TenantResponseDto>>), 200)]
        public async Task<ActionResult<ApiResponse<PagedResponse<TenantResponseDto>>>> GetTenants(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var tenants = await _tenantService.GetTenantsAsync(pageNumber, pageSize);
            return Ok(ApiResponse<PagedResponse<TenantResponseDto>>.CreateSuccess(tenants));
        }

        /// <summary>
        /// Gets a tenant by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TenantResponseDto>), 200)]
        public async Task<ActionResult<ApiResponse<TenantResponseDto>>> GetTenant(Guid id)
        {
            var tenant = await _tenantService.GetTenantByIdAsync(id);
            return Ok(ApiResponse<TenantResponseDto>.CreateSuccess(tenant));
        }
    }
} 