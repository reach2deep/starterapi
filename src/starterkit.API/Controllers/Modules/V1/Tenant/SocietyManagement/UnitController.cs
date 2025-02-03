using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
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
        [ProducesResponseType(typeof(ApiResponse<UnitResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _unitService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all units
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _unitService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a paged list of units
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<UnitResponse>>), 200)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _unitService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets units data optimized for dropdown/lookup controls
        /// </summary>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<LookupResponse<LookupDto>>), 200)]
        public async Task<IActionResult> GetLookup([FromQuery] LookupRequest request)
        {
            var result = await _unitService.GetLookupAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Gets all units for a specific floor
        /// </summary>
        [HttpGet("floor/{floorId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UnitResponse>>), 200)]
        public async Task<IActionResult> GetByFloorId(Guid floorId)
        {
            var result = await _unitService.GetByFloorIdAsync(floorId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new unit
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UnitResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateUnitRequest request)
        {
            var result = await _unitService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing unit
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UnitResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _unitService.UpdateAsync(request);
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
    }
} 