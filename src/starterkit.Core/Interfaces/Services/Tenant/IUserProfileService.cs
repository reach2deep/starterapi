using starterkit.Core.DTOs.Tenant;

namespace starterkit.Core.Interfaces.Services.Tenant
{
    public interface IUserProfileService
    {
        Task<UserProfileResponseDto> GetMyProfileAsync(Guid userId);
        Task<UserProfileResponseDto> GetProfileAsync(Guid id);
        Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequestDto request);
    }
} 
