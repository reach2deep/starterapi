using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using starterkit.Core.DTOs.Tenant;
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
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateUserProfileRequestDto> _updateProfileValidator;

        public UserProfileController(
            Func<string, TenantDbContext> tenantDbFactory,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IValidator<UpdateUserProfileRequestDto> updateProfileValidator)
        {
            _tenantDbFactory = tenantDbFactory;
            _tenantId = httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
            _mapper = mapper;
            _updateProfileValidator = updateProfileValidator;
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

                return Ok(_mapper.Map<UserProfileResponseDto>(profile));
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
                // Validate request
                var validationResult = await _updateProfileValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new 
                    { 
                        error = "Validation failed",
                        details = validationResult.Errors.Select(e => new 
                        { 
                            field = e.PropertyName,
                            error = e.ErrorMessage 
                        })
                    });
                }

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

                // Update profile using AutoMapper
                _mapper.Map(request, existingProfile);
                existingProfile.UpdatedBy = Guid.Parse(userId);

                // Update address if provided
                if (request.Address != null)
                {
                    if (existingProfile.Address == null)
                    {
                        existingProfile.Address = new Address();
                    }
                    _mapper.Map(request.Address, existingProfile.Address);
                    existingProfile.Address.UpdatedBy = Guid.Parse(userId);
                }

                await context.SaveChangesAsync();
                return Ok(_mapper.Map<UserProfileResponseDto>(existingProfile));
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

                return Ok(_mapper.Map<UserProfileResponseDto>(profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the profile", details = ex.Message });
            }
        }
    }
} 