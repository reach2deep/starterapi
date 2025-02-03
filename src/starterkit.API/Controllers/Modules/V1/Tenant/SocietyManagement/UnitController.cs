using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.SocietyManagement
{
    /// <summary>
    /// Controller for managing units within floors
    /// </summary>
    [Route("api/v1/tenant/units")]
    [Authorize]
    public class UnitController : BaseApiController
    {
        private readonly IUnitService _unitService;

        public UnitController(IUnitService unitService)
        {
            _unitService = unitService;
        }

        /// <summary>
        /// Gets a unit by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all units
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Unit>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _unitService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets all units for a specific floor
        /// </summary>
        [HttpGet("floor/{floorId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Unit>>), 200)]
        public async Task<IActionResult> GetByFloorId(Guid floorId)
        {
            var result = await _unitService.GetByFloorIdAsync(floorId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all units for a specific block
        /// </summary>
        [HttpGet("block/{blockId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Unit>>), 200)]
        public async Task<IActionResult> GetByBlockId(Guid blockId)
        {
            var result = await _unitService.GetByBlockIdAsync(blockId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all units for a specific society
        /// </summary>
        [HttpGet("society/{societyId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<Unit>>), 200)]
        public async Task<IActionResult> GetBySocietyId(Guid societyId)
        {
            var result = await _unitService.GetBySocietyIdAsync(societyId);
            return Ok(result);
        }

        /// <summary>
        /// Gets a unit by its unit number within a society
        /// </summary>
        [HttpGet("society/{societyId}/unit-number/{unitNumber}")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), 200)]
        public async Task<IActionResult> GetByUnitNumber(Guid societyId, string unitNumber)
        {
            var result = await _unitService.GetByUnitNumberAsync(societyId, unitNumber);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new unit
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Unit>), 200)]
        public async Task<IActionResult> Create([FromBody] Unit unit)
        {
            var result = await _unitService.CreateAsync(unit);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing unit
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] Unit unit)
        {
            if (id != unit.Id)
                return BadRequest("ID mismatch");

            var result = await _unitService.UpdateAsync(unit);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a unit
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets a unit with all its related details
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ApiResponse<Unit>), 200)]
        public async Task<IActionResult> GetByIdWithDetails(Guid id)
        {
            var result = await _unitService.GetByIdWithDetailsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Checks if a unit number is unique within a society
        /// </summary>
        [HttpGet("society/{societyId}/number-unique")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> IsUnitNumberUniqueInSociety(Guid societyId, [FromQuery] string unitNumber, [FromQuery] Guid? excludeId = null)
        {
            var result = await _unitService.IsUnitNumberUniqueInSocietyAsync(societyId, unitNumber, excludeId);
            return Ok(result);
        }
    }
} 