using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.SocietyManagement
{
    /// <summary>
    /// Controller for managing unit ownership records
    /// </summary>
    [Route("api/v1/tenant/unit-ownerships")]
    [Authorize]
    public class UnitOwnershipController : BaseApiController
    {
        private readonly IUnitOwnershipService _ownershipService;

        public UnitOwnershipController(IUnitOwnershipService ownershipService)
        {
            _ownershipService = ownershipService;
        }

        /// <summary>
        /// Gets an ownership record by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UnitOwnershipResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _ownershipService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all ownership records
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitOwnershipResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ownershipService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets all ownership records for a specific unit
        /// </summary>
        [HttpGet("unit/{unitId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitOwnershipResponse>>), 200)]
        public async Task<IActionResult> GetByUnitId(Guid unitId)
        {
            var result = await _ownershipService.GetByUnitIdAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all ownership records for a specific owner
        /// </summary>
        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitOwnershipResponse>>), 200)]
        public async Task<IActionResult> GetByOwnerId(Guid ownerId)
        {
            var result = await _ownershipService.GetByOwnerIdAsync(ownerId);
            return Ok(result);
        }

        /// <summary>
        /// Gets current ownership record for a specific unit
        /// </summary>
        [HttpGet("unit/{unitId}/current")]
        [ProducesResponseType(typeof(ApiResponse<UnitOwnershipResponse>), 200)]
        public async Task<IActionResult> GetCurrentOwnership(Guid unitId)
        {
            var result = await _ownershipService.GetCurrentOwnershipAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new ownership record
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UnitOwnershipResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateUnitOwnershipRequest request)
        {
            var result = await _ownershipService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing ownership record
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UnitOwnershipResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitOwnershipRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _ownershipService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes an ownership record
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _ownershipService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Transfers ownership of a unit from one owner to another
        /// </summary>
        [HttpPost("transfer")]
        [ProducesResponseType(typeof(ApiResponse<UnitOwnershipResponse>), 200)]
        public async Task<IActionResult> TransferOwnership([FromQuery] Guid unitId, [FromQuery] Guid currentOwnerId, [FromQuery] Guid newOwnerId, [FromQuery] DateTime transferDate)
        {
            var result = await _ownershipService.TransferOwnershipAsync(unitId, currentOwnerId, newOwnerId, transferDate);
            return Ok(result);
        }
    }
} 