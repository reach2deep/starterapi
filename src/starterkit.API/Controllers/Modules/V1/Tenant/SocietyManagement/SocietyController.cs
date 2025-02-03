using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.Modules.Tenant.SocietyManagement.Interfaces.Services;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Core.Modules.Common;

namespace starterkit.API.Controllers.Modules.V1.Tenant.SocietyManagement
{
    /// <summary>
    /// Controller for managing societies
    /// </summary>
    [Route("api/v1/tenant/societies")]
    [Authorize]
    public class SocietyController : BaseApiController
    {
        private readonly ISocietyService _societyService;

        public SocietyController(ISocietyService societyService)
        {
            _societyService = societyService;
        }

        /// <summary>
        /// Gets a society by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _societyService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all societies
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SocietyResponse>>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _societyService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Creates a new society
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
        public async Task<IActionResult> Create([FromBody] CreateSocietyRequest request)
        {
            var result = await _societyService.CreateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing society
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSocietyRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var result = await _societyService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a society
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _societyService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets a society with all its related details
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
        public async Task<IActionResult> GetByIdWithDetails(Guid id)
        {
            var result = await _societyService.GetByIdWithDetailsAsync(id);
            return Ok(result);
        }

  
    }
} 