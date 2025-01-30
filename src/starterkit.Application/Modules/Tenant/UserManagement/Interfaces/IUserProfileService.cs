

using starterkit.Application.Modules.Tenant.UserManagement.DTOs;

namespace starterkit.Application.Modules.Tenant.UserManagement.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileResponseDto> GetMyProfileAsync(Guid userId);
        Task<UserProfileResponseDto> GetProfileAsync(Guid id);
        Task<UserProfileResponseDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequestDto request);
    }
}
