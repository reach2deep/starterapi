using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly string _tenantId;

        public UserProfileController(
            Func<string, TenantDbContext> tenantDbFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantDbFactory = tenantDbFactory;
            _tenantId = httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserProfile>> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            using var context = _tenantDbFactory(_tenantId);
            var profile = await context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfile profile)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            using var context = _tenantDbFactory(_tenantId);
            var existingProfile = await context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

            if (existingProfile == null)
            {
                return NotFound();
            }

            // Update only allowed fields
            existingProfile.Address = profile.Address;
            existingProfile.City = profile.City;
            existingProfile.Country = profile.Country;
            existingProfile.PostalCode = profile.PostalCode;
            existingProfile.DateOfBirth = profile.DateOfBirth;
            existingProfile.ProfilePictureUrl = profile.ProfilePictureUrl;
            existingProfile.UpdatedBy = Guid.Parse(userId);
            existingProfile.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return Ok(existingProfile);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserProfile>> GetProfile(Guid id)
        {
            using var context = _tenantDbFactory(_tenantId);
            var profile = await context.UserProfiles
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }
    }
} 