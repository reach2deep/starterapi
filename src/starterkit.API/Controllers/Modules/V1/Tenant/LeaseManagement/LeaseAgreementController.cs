using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using starterkit.API.Controllers;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.LeaseManagement
{
    /// <summary>
    /// API endpoints for managing lease agreements
    /// </summary>
    
    [Route("api/v1/tenant/lease-agreements")]
    [Authorize]
    public class LeaseAgreementController : BaseApiController
    {
        private readonly ILeaseAgreementService _leaseAgreementService;

        public LeaseAgreementController(ILeaseAgreementService leaseAgreementService)
        {
            _leaseAgreementService = leaseAgreementService;
        }

        /// <summary>
        /// Gets all lease agreements
        /// </summary>
        /// <returns>List of lease agreements</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaseAgreementResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _leaseAgreementService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a lease agreement by ID
        /// </summary>
        /// <param name="id">The lease agreement ID</param>
        /// <returns>The lease agreement details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LeaseAgreementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _leaseAgreementService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all lease agreements for a specific unit
        /// </summary>
        /// <param name="unitId">The unit ID</param>
        /// <returns>List of lease agreements for the unit</returns>
        [HttpGet("by-unit/{unitId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaseAgreementResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByUnitId(Guid unitId)
        {
            var result = await _leaseAgreementService.GetByUnitIdAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all lease agreements for a specific owner
        /// </summary>
        /// <param name="ownerId">The owner ID</param>
        /// <returns>List of lease agreements for the owner</returns>
        [HttpGet("by-owner/{ownerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaseAgreementResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByOwnerId(Guid ownerId)
        {
            var result = await _leaseAgreementService.GetByOwnerIdAsync(ownerId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all lease agreements for a specific tenant
        /// </summary>
        /// <param name="tenantId">The tenant ID</param>
        /// <returns>List of lease agreements for the tenant</returns>
        [HttpGet("by-tenant/{tenantId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LeaseAgreementResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByTenantId(Guid tenantId)
        {
            var result = await _leaseAgreementService.GetByTenantIdAsync(tenantId);
            return Ok(result);
        }

        /// <summary>
        /// Gets the active lease agreement for a specific unit
        /// </summary>
        /// <param name="unitId">The unit ID</param>
        /// <returns>The active lease agreement for the unit</returns>
        [HttpGet("active/by-unit/{unitId}")]
        [ProducesResponseType(typeof(ApiResponse<LeaseAgreementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveLeaseForUnit(Guid unitId)
        {
            var result = await _leaseAgreementService.GetActiveLeaseForUnitAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paged list of lease agreements
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>Paged list of lease agreements</returns>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<LeaseAgreementResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _leaseAgreementService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new lease agreement
        /// </summary>
        /// <param name="request">The lease agreement details</param>
        /// <returns>The created lease agreement</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<LeaseAgreementResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateLeaseAgreementRequest request)
        {
            var result = await _leaseAgreementService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Updates an existing lease agreement
        /// </summary>
        /// <param name="id">The lease agreement ID</param>
        /// <param name="request">The updated lease agreement details</param>
        /// <returns>The updated lease agreement</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<LeaseAgreementResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLeaseAgreementRequest request)
        {
            request.Id = id;
            var result = await _leaseAgreementService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a lease agreement
        /// </summary>
        /// <param name="id">The lease agreement ID</param>
        /// <returns>True if deleted successfully</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _leaseAgreementService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets lease agreements data optimized for dropdown/lookup controls
        /// </summary>
        /// <param name="request">The lookup request parameters</param>
        /// <returns>Lookup data for lease agreements</returns>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<LookupResponse<LookupDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLookup([FromQuery] LookupRequest request)
        {
            var result = await _leaseAgreementService.GetLookupAsync(request);
            return Ok(result);
        }
    }
} 