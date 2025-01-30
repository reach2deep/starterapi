
using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Core.Modules.Tenant;

namespace starterkit.Application.Modules.Tenant.UserManagement.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _profileRepository;

        public UserProfileService(IUserProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<UserProfileResponseDto> GetMyProfileAsync(Guid userId)
        {
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