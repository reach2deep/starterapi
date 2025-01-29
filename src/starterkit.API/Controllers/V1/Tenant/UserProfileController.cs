using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using starterkit.Application.DTOs.Tenant;
using starterkit.Core.Entities.Tenant;
using starterkit.Infrastructure.Data.TenantDb;

namespace starterkit.API.Controllers.V1.Tenant
{
    [ApiController]
    [Route("api/v1/tenant/profiles")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly Func<string, TenantDbContext> _tenantDbFactory;
        private readonly string? _tenantId;

        public UserProfileController(
            Func<string, TenantDbContext> tenantDbFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantDbFactory = tenantDbFactory;
            _tenantId = httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileResponseDto>> GetMyProfile()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                using var context = _tenantDbFactory(_tenantId);
                var profile = await context.UserProfiles
                    .Include(p => p.Address)
                    .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

                if (profile == null)
                {
                    return NotFound(new { error = "Profile not found" });
                }

                return Ok(MapToResponseDto(profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the profile", details = ex.Message });
            }
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserProfileResponseDto>> UpdateMyProfile([FromBody] UpdateUserProfileRequestDto request)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new { error = "User not authenticated" });
                }

                using var context = _tenantDbFactory(_tenantId);
                var existingProfile = await context.UserProfiles
                    .Include(p => p.Address)
                    .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

                if (existingProfile == null)
                {
                    return NotFound(new { error = "Profile not found" });
                }

                // Update address if provided
                if (request.Address != null)
                {
                    if (existingProfile.Address == null)
                    {
                        existingProfile.Address = new Address();
                    }

                    existingProfile.Address.StreetAddress = request.Address.StreetAddress;
                    existingProfile.Address.City = request.Address.City;
                    existingProfile.Address.Country = request.Address.Country;
                    existingProfile.Address.PostalCode = request.Address.PostalCode;
                    existingProfile.Address.State = request.Address.State;
                    existingProfile.Address.UpdatedBy = Guid.Parse(userId);
                    existingProfile.Address.UpdatedAt = DateTime.UtcNow;
                }

                // Update profile fields
                existingProfile.DateOfBirth = request.DateOfBirth;
                existingProfile.ProfilePictureUrl = request.ProfilePictureUrl;
                existingProfile.UpdatedBy = Guid.Parse(userId);
                existingProfile.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();
                return Ok(MapToResponseDto(existingProfile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the profile", details = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserProfileResponseDto>> GetProfile(Guid id)
        {
            try
            {
                using var context = _tenantDbFactory(_tenantId);
                var profile = await context.UserProfiles
                    .Include(p => p.Address)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (profile == null)
                {
                    return NotFound(new { error = "Profile not found" });
                }

                return Ok(MapToResponseDto(profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the profile", details = ex.Message });
            }
        }

        private static UserProfileResponseDto MapToResponseDto(UserProfile profile)
        {
            return new UserProfileResponseDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                DateOfBirth = profile.DateOfBirth,
                ProfilePictureUrl = profile.ProfilePictureUrl,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                Address = profile.Address == null ? null : new AddressDto
                {
                    Id = profile.Address.Id,
                    StreetAddress = profile.Address.StreetAddress,
                    City = profile.Address.City,
                    Country = profile.Address.Country,
                    PostalCode = profile.Address.PostalCode,
                    State = profile.Address.State
                }
            };
        }
    }
} 