using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.SocietyManagement
{
    /// <summary>
    /// Controller for managing floors within blocks
    /// </summary>
    [Route("api/v1/tenant/floors")]
    [Authorize]
    public class FloorController : BaseApiController
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        /// <summary>
        /// Gets a floor by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FloorResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _floorService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all floors
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FloorResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _floorService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets all floors for a specific block
        /// </summary>
        [HttpGet("block/{blockId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FloorResponse>>), 200)]
        public async Task<IActionResult> GetByBlockId(Guid blockId)
        {
            var result = await _floorService.GetByBlockIdAsync(blockId);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new floor
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FloorResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateFloorRequest request)
        {
            var result = await _floorService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing floor
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FloorResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFloorRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _floorService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a floor
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _floorService.DeleteAsync(id);
            return Ok(result);
        }

    }
} 