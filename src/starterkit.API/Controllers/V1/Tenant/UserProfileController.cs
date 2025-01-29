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
        private readonly string? _tenantId;

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
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileUpdateModel model)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }

            using var context = _tenantDbFactory(_tenantId);
            var existingProfile = await context.UserProfiles
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

            if (existingProfile == null)
            {
                return NotFound();
            }

            // Update address if provided
            if (model.Address != null)
            {
                if (existingProfile.Address == null)
                {
                    existingProfile.Address = new Address();
                }

                existingProfile.Address.StreetAddress = model.Address.StreetAddress;
                existingProfile.Address.City = model.Address.City;
                existingProfile.Address.Country = model.Address.Country;
                existingProfile.Address.PostalCode = model.Address.PostalCode;
                existingProfile.Address.State = model.Address.State;
                existingProfile.Address.UpdatedBy = Guid.Parse(userId);
                existingProfile.Address.UpdatedAt = DateTime.UtcNow;
            }

            // Update profile fields
            existingProfile.DateOfBirth = model.DateOfBirth;
            existingProfile.ProfilePictureUrl = model.ProfilePictureUrl;
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
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }
    }

    public class UserProfileUpdateModel
    {
        public AddressUpdateModel? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public class AddressUpdateModel
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string? State { get; set; }
    }
} 