using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.SocietyManagement
{
    /// <summary>
    /// Controller for managing unit resident records
    /// </summary>
    [Route("api/v1/tenant/unit-residents")]
    [Authorize]
    public class UnitResidentController : BaseApiController
    {
        private readonly IUnitResidentService _residentService;

        public UnitResidentController(IUnitResidentService residentService)
        {
            _residentService = residentService;
        }

        /// <summary>
        /// Gets a resident record by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _residentService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all resident records
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitResidentResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _residentService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets all resident records for a specific unit
        /// </summary>
        [HttpGet("unit/{unitId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitResidentResponse>>), 200)]
        public async Task<IActionResult> GetByUnitId(Guid unitId)
        {
            var result = await _residentService.GetByUnitIdAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all units where a specific user is a resident
        /// </summary>
        [HttpGet("resident/{residentId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitResidentResponse>>), 200)]
        public async Task<IActionResult> GetByResidentId(Guid residentId)
        {
            var result = await _residentService.GetByResidentIdAsync(residentId);
            return Ok(result);
        }

        /// <summary>
        /// Gets current resident records for a specific unit
        /// </summary>
        [HttpGet("unit/{unitId}/current")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitResidentResponse>>), 200)]
        public async Task<IActionResult> GetCurrentResidents(Guid unitId)
        {
            var result = await _residentService.GetCurrentResidentsAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Gets the primary resident record for a specific unit
        /// </summary>
        [HttpGet("unit/{unitId}/primary")]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> GetPrimaryResident(Guid unitId)
        {
            var result = await _residentService.GetPrimaryResidentAsync(unitId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new resident record
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateUnitResidentRequest request)
        {
            var result = await _residentService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing resident record
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitResidentRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _residentService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a resident record
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _residentService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets a resident record with all its related details
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> GetByIdWithDetails(Guid id)
        {
            var result = await _residentService.GetByIdWithDetailsAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Checks if a user is currently a resident in any unit
        /// </summary>
        [HttpGet("resident/{residentId}/is-active")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> IsActiveResident(Guid residentId)
        {
            var result = await _residentService.IsActiveResidentAsync(residentId);
            return Ok(result);
        }

        /// <summary>
        /// Sets a resident as the primary resident for a unit
        /// </summary>
        [HttpPost("unit/{unitId}/set-primary/{residentId}")]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> SetPrimaryResident(Guid unitId, Guid residentId)
        {
            var result = await _residentService.SetPrimaryResidentAsync(unitId, residentId);
            return Ok(result);
        }

        /// <summary>
        /// Moves a resident out of a unit
        /// </summary>
        [HttpPost("move-out")]
        [ProducesResponseType(typeof(ApiResponse<UnitResidentResponse>), 200)]
        public async Task<IActionResult> MoveOutResident([FromQuery] Guid unitId, [FromQuery] Guid residentId, [FromQuery] DateTime moveOutDate)
        {
            var result = await _residentService.MoveOutResidentAsync(unitId, residentId, moveOutDate);
            return Ok(result);
        }
    }
} 