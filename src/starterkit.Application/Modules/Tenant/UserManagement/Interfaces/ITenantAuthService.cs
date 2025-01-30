namespace starterkit.starterkit.Application.Modules.Tenant.UserManagement.Interfaces
{
    public interface ITenantAuthService
    {
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
        Task<bool> ValidateAccessTokenAsync(string accessToken);
    }
}