using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.API.Controllers;
using starterkit.Core.DTOs.Tenant;
using starterkit.Core.Interfaces.Services.Tenant;
using starterkit.Core.Models;

namespace starterkit.API.Controllers.V1.Tenant
{
    /// <summary>
    /// Controller for managing user profiles in tenant context
    /// </summary>
    [Route("api/v1/tenant/profiles")]
    [Authorize]
    public class UserProfileController : BaseApiController
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
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized<UserProfileResponseDto>();
            }

            var result = await _userProfileService.GetMyProfileAsync(userId.Value);
            return Ok(ApiResponse<UserProfileResponseDto>.CreateSuccess(result));
        }

        /// <summary>
        /// Update the profile of the currently authenticated user
        /// </summary>
        [HttpPut("me")]
        public async Task<ActionResult<ApiResponse<UserProfileResponseDto>>> UpdateMyProfile(
            [FromBody] UpdateUserProfileRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized<UserProfileResponseDto>();
            }

            var result = await _userProfileService.UpdateProfileAsync(userId.Value, request);
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