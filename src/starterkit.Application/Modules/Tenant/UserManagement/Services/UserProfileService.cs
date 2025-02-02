using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Core.Modules.Tenant;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace starterkit.Application.Modules.Tenant.UserManagement.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserProfileService(
            IUserProfileRepository profileRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _profileRepository = profileRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserProfileResponseDto> GetMyProfileAsync(Guid userId)
        {
            // Check if user is root admin from JWT claims
            var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
            
            if (role == "RootAdmin")
            {
                // For root admin, return a default profile
                return new UserProfileResponseDto
                {
                    Id = userId,
                    UserId = userId,
                    FirstName = "Root",
                    LastName = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            // For non-root admin users, get profile from tenant database
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profile not found");
            }

            return MapToResponseDto(profile);
        }

        public async Task<UserProfileResponseDto> GetProfileAsync(Guid id)
        {
            var profile = await _profileRepository.GetByIdAsync(id);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profile not found");
            }

            return MapToResponseDto(profile);
        }

        public async Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequestDto request)
        {
            var profile = await _profileRepository.GetByUserIdAsync(userId);
            if (profile == null)
            {
                throw new KeyNotFoundException("Profile not found");
            }

            // Update address if provided
            if (request.Address != null)
            {
                if (profile.Address == null)
                {
                    profile.Address = new Address();
                }

                profile.Address.StreetAddress = request.Address.StreetAddress;
                profile.Address.City = request.Address.City;
                profile.Address.Country = request.Address.Country;
                profile.Address.PostalCode = request.Address.PostalCode;
                profile.Address.State = request.Address.State;
                profile.Address.UpdatedBy = userId;
                profile.Address.UpdatedAt = DateTime.UtcNow;
            }

            // Update profile fields
            profile.FirstName = request.FirstName;
            profile.LastName = request.LastName;
            profile.DateOfBirth = request.DateOfBirth;
            profile.ProfilePictureUrl = request.ProfilePictureUrl;
            profile.UpdatedBy = userId;
            profile.UpdatedAt = DateTime.UtcNow;

            await _profileRepository.UpdateAsync(profile);
            return MapToResponseDto(profile);
        }

        private static UserProfileResponseDto MapToResponseDto(UserProfile profile)
        {
            return new UserProfileResponseDto
            {
                Id = profile.Id,
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
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