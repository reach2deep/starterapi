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
    /// API endpoints for managing rent payments
    /// </summary>
   
    [Authorize]
    [Route("api/v1/tenant/rent-payments")]
    public class RentPaymentController : BaseApiController
    {
        private readonly IRentPaymentService _rentPaymentService;
        private readonly ILeaseAgreementService _leaseAgreementService;

        public RentPaymentController(IRentPaymentService rentPaymentService, ILeaseAgreementService leaseAgreementService)
        {
            _rentPaymentService = rentPaymentService;
            _leaseAgreementService = leaseAgreementService;
        }

        /// <summary>
        /// Gets all rent payments
        /// </summary>
        /// <returns>List of rent payments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RentPaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _rentPaymentService.GetAllAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a rent payment by ID
        /// </summary>
        /// <param name="id">The rent payment ID</param>
        /// <returns>The rent payment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RentPaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _rentPaymentService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets all rent payments for a specific lease agreement
        /// </summary>
        /// <param name="leaseAgreementId">The lease agreement ID</param>
        /// <returns>List of rent payments for the lease agreement</returns>
        [HttpGet("by-lease/{leaseAgreementId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RentPaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByLeaseAgreementId(Guid leaseAgreementId)
        {
            var result = await _rentPaymentService.GetByLeaseAgreementIdAsync(leaseAgreementId);
            return Ok(result);
        }

        /// <summary>
        /// Gets all payments with a specific status
        /// </summary>
        /// <param name="status">The payment status</param>
        /// <returns>List of rent payments with the specified status</returns>
        [HttpGet("by-status/{status}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RentPaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var result = await _rentPaymentService.GetByStatusAsync(status);
            return Ok(result);
        }

        /// <summary>
        /// Gets all payments due between specific dates
        /// </summary>
        /// <param name="startDate">The start date</param>
        /// <param name="endDate">The end date</param>
        /// <returns>List of rent payments due within the date range</returns>
        [HttpGet("by-due-date")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RentPaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByDueDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _rentPaymentService.GetByDueDateRangeAsync(startDate, endDate);
            return Ok(result);
        }

        /// <summary>
        /// Gets all overdue payments
        /// </summary>
        /// <returns>List of overdue rent payments</returns>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RentPaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOverduePayments()
        {
            var result = await _rentPaymentService.GetOverduePaymentsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Gets a paged list of rent payments
        /// </summary>
        /// <param name="pageNumber">The page number</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>Paged list of rent payments</returns>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<RentPaymentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _rentPaymentService.GetPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Creates a new rent payment
        /// </summary>
        /// <param name="request">The rent payment details</param>
        /// <returns>The created rent payment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RentPaymentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateRentPaymentRequest request)
        {
            var result = await _rentPaymentService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Creates a new rent payment for a specific unit's active lease
        /// </summary>
        /// <param name="unitId">The ID of the unit</param>
        /// <param name="request">The rent payment details</param>
        /// <returns>The created rent payment</returns>
        [HttpPost("by-unit/{unitId}")]
        [ProducesResponseType(typeof(ApiResponse<RentPaymentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateByUnitId(Guid unitId, [FromBody] CreateRentPaymentByUnitRequest request)
        {
            var result = await _rentPaymentService.CreateByUnitIdAsync(unitId, request);
            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
        }

        /// <summary>
        /// Updates an existing rent payment
        /// </summary>
        /// <param name="id">The rent payment ID</param>
        /// <param name="request">The updated rent payment details</param>
        /// <returns>The updated rent payment</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<RentPaymentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRentPaymentRequest request)
        {
            request.Id = id;
            var result = await _rentPaymentService.UpdateAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a rent payment
        /// </summary>
        /// <param name="id">The rent payment ID</param>
        /// <returns>True if deleted successfully</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _rentPaymentService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gets rent payments data optimized for dropdown/lookup controls
        /// </summary>
        /// <param name="request">The lookup request parameters</param>
        /// <returns>Lookup data for rent payments</returns>
        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<LookupResponse<LookupDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetLookup([FromQuery] LookupRequest request)
        {
            var result = await _rentPaymentService.GetLookupAsync(request);
            return Ok(result);
        }
    }
} 