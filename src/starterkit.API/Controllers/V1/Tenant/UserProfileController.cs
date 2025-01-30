using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Core.DTOs.Tenant;
using starterkit.Core.Interfaces.Services.Tenant;
using starterkit.Core.Models;

namespace starterkit.API.Controllers.V1.Tenant
{
    /// <summary>
    /// Controller for managing user profiles in tenant context
    /// </summary>
    [ApiController]
    [Route("api/v1/tenant/profiles")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Get the profile of the currently authenticated user
        /// </summary>
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(ApiResponse<UserProfileResponseDto>.CreateError(
                    message: "User not authenticated",
                    code: "UNAUTHORIZED"
                ));
            }

            var result = await _userProfileService.GetMyProfileAsync(Guid.Parse(userId));
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Update the profile of the currently authenticated user
        /// </summary>
        [HttpPut("me")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> UpdateMyProfile(
            [FromBody] UpdateUserProfileRequestDto request)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(ApiResponse<UserProfileResponseDto>.CreateError(
                    message: "User not authenticated",
                    code: "UNAUTHORIZED"
                ));
            }

            var result = await _userProfileService.UpdateProfileAsync(Guid.Parse(userId), request);
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Get a user profile by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> GetProfile(Guid id)
        {
            var result = await _userProfileService.GetProfileAsync(id);
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }
    }
} 