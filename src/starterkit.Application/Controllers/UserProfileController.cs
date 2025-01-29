using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using starterkit.Core.DTOs.Tenant;
using starterkit.Core.Interfaces.Services.Tenant;

namespace starterkit.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileResponseDto>> GetMyProfile()
        {
            try
            {
                var userId = Guid.Parse(User.Identity?.Name ?? throw new UnauthorizedAccessException("User not authenticated"));
                var profile = await _userProfileService.GetMyProfileAsync(userId);
                return Ok(profile);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Profile not found" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile(Guid id)
        {
            try
            {
                var profile = await _userProfileService.GetProfileAsync(id);
                return Ok(profile);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Profile not found" });
            }
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserProfileResponseDto>> UpdateMyProfile([FromBody] UpdateUserProfileRequestDto request)
        {
            try
            {
                var userId = Guid.Parse(User.Identity?.Name ?? throw new UnauthorizedAccessException("User not authenticated"));
                var updatedProfile = await _userProfileService.UpdateProfileAsync(userId, request);
                return Ok(updatedProfile);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Profile not found" });
            }
        }
    }
} 