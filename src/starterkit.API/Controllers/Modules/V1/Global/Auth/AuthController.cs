using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.DTOs.Global.Auth;
using starterkit.Application.Interfaces.Services.Global;
using starterkit.Core.Models;

namespace starterkit.starterkit.API.Controllers.Modules.V1.Global.Auth
{
    /// <summary>
    /// Controller responsible for handling global authentication operations
    /// such as user login and tenant selection.
    /// </summary>
    [ApiController]
    [Route("api/v1/global/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IGlobalAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the AuthController
        /// </summary>
        /// <param name="authService">The global authentication service</param>
        public AuthController(IGlobalAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns their global access token along with available tenants
        /// </summary>
        /// <param name="request">Login credentials containing email and password</param>
        /// <returns>
        /// Success Response: Global access token, user info, and available tenants
        /// Error Response: Handled by GlobalExceptionHandlingMiddleware
        /// </returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<GlobalLoginResponseDto>>> Login(
            [FromBody] GlobalLoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            return Ok(ApiResponse<GlobalLoginResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Selects a tenant for an authenticated user and returns a tenant-specific access token
        /// </summary>
        /// <param name="request">Tenant selection details including tenant ID</param>
        /// <returns>
        /// Success Response: Tenant-specific access token and tenant info
        /// Error Response: Handled by GlobalExceptionHandlingMiddleware
        /// </returns>
        [HttpPost("select-tenant")]
        public async Task<ActionResult<ApiResponse<TenantSelectionResponseDto>>> SelectTenant(
            [FromBody] TenantSelectionRequestDto request)
        {
            var result = await _authService.SelectTenantAsync(request);

            return Ok(ApiResponse<TenantSelectionResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Authenticates a user and automatically selects their first available tenant
        /// </summary>
        /// <param name="request">Login credentials containing email and password</param>
        /// <returns>
        /// Success Response: Tenant-specific access token and tenant info
        /// Error Response: If no tenants available or other authentication errors
        /// </returns>
        [HttpPost("login-with-tenant")]
        public async Task<ActionResult<ApiResponse<TenantSelectionResponseDto>>> LoginWithFirstTenant(
            [FromBody] GlobalLoginRequestDto request)
        {
            // First, perform normal login
            var loginResult = await _authService.LoginAsync(request);

            // Check if user has any available tenants
            if (loginResult.AvailableTenants == null || !loginResult.AvailableTenants.Any())
            {
                return BadRequest(ApiResponse<TenantSelectionResponseDto>.CreateError(
                    message: "No available tenants for this user",
                    code: "NO_TENANTS_AVAILABLE"
                ));
            }

            // Select the first available tenant
            var tenantSelectionRequest = new TenantSelectionRequestDto
            {
                BaseToken = loginResult.BaseToken,
                TenantId = loginResult.AvailableTenants.First().TenantId
            };

            // Perform tenant selection
            var tenantResult = await _authService.SelectTenantAsync(tenantSelectionRequest);

            return Ok(ApiResponse<TenantSelectionResponseDto>.CreateSuccess(tenantResult));
        }
    }
}