

using starterkit.starterkit.Application.Modules.Global.Auth.DTOs;
using starterkit.starterkit.Core.Modules.Global;

namespace starterkit.starterkit.Application.Modules.Global.Auth.Interfaces
{
    public interface IGlobalAuthService
    {
        Task<GlobalLoginResponseDto> LoginAsync(GlobalLoginRequestDto request);
        Task<TenantSelectionResponseDto> SelectTenantAsync(TenantSelectionRequestDto request);
        Task<GlobalUser> ValidateBaseTokenAsync(string baseToken);
    }
}