using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.SocietyManagement
{
    /// <summary>
    /// Controller for managing blocks within societies
    /// </summary>
    [Route("api/v1/tenant/blocks")]
    [Authorize]
    public class BlockController : BaseApiController
    {
        private readonly IBlockService _blockService;

        public BlockController(IBlockService blockService)
        {
            _blockService = blockService;
        }

        /// <summary>
        /// Gets a block by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BlockResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _blockService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all blocks
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BlockResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _blockService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new block
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<BlockResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateBlockRequest request)
        {
            var result = await _blockService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing block
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<BlockResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBlockRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _blockService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a block
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _blockService.DeleteAsync(id);
            return Ok(result);
        }
    }
} 