using Microsoft.AspNetCore.Mvc;
using starterkit.Application.DTOs.Global.Auth;
using starterkit.Application.Services.Global;

namespace starterkit.API.Controllers.V1.Global
{
    [ApiController]
    [Route("api/v1/global/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IGlobalAuthService _authService;

        public AuthController(IGlobalAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<GlobalLoginResponseDto>> Login([FromBody] GlobalLoginRequestDto request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("select-tenant")]
        public async Task<ActionResult<TenantSelectionResponseDto>> SelectTenant([FromBody] TenantSelectionRequestDto request)
        {
            try
            {
                var response = await _authService.SelectTenantAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
} 