using starterkit.Application.DTOs.Global.Auth;
using starterkit.Core.Entities.Global;

namespace starterkit.Application.Interfaces.Services.Global
{
    public interface IGlobalAuthService
    {
        Task<GlobalLoginResponseDto> LoginAsync(GlobalLoginRequestDto request);
        Task<TenantSelectionResponseDto> SelectTenantAsync(TenantSelectionRequestDto request);
        Task<GlobalUser> ValidateBaseTokenAsync(string baseToken);
    }
} 