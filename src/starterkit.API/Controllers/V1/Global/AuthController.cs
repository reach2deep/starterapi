using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Application.DTOs.Global.Auth;
using starterkit.Application.Interfaces.Services.Global;
using starterkit.Core.Models;

namespace starterkit.API.Controllers.V1.Global
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
    }
} 