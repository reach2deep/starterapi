using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Application.Persistence;
using starterkit.Core.Modules.Tenant;
using System.Security.Claims;

namespace starterkit.Application.Modules.Tenant.UserManagement.Services
{
    public class TenantAuthService : ITenantAuthService
    {
        private readonly Func<string, ITenantDbContext> _tenantDbFactory;
        private readonly IConfiguration _configuration;
        private readonly string _tenantId;

        public TenantAuthService(
            Func<string, ITenantDbContext> tenantDbFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _tenantDbFactory = tenantDbFactory;
            _configuration = configuration;
            _tenantId = httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            using var context = _tenantDbFactory(_tenantId);
            var storedRefreshToken = await context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            if (storedRefreshToken.ExpiresAt < DateTime.UtcNow || storedRefreshToken.IsRevoked)
            {
                throw new UnauthorizedAccessException("Refresh token has expired or been revoked");
            }

            // Generate new tokens
            var newAccessToken = GenerateAccessToken(storedRefreshToken.User);
            var newRefreshToken = GenerateRefreshToken();

            // Update refresh token in database
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.ReplacedByToken = newRefreshToken;
            storedRefreshToken.RevokedReason = "Refresh token rotation";

            var newRefreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                UserId = storedRefreshToken.UserId,
                ExpiresAt = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("JWT:RefreshTokenExpirationDays")),
                CreatedBy = storedRefreshToken.UserId
            };

            context.RefreshTokens.Add(newRefreshTokenEntity);
            await context.SaveChangesAsync();

            return (newAccessToken, newRefreshToken);
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            using var context = _tenantDbFactory(_tenantId);
            var storedRefreshToken = await context.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (storedRefreshToken != null && !storedRefreshToken.IsRevoked)
            {
                storedRefreshToken.IsRevoked = true;
                storedRefreshToken.RevokedReason = "Revoked by user";
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateAccessTokenAsync(string accessToken)
        {
            // Implement token validation logic here
            // This is just a placeholder
            return true;
        }

        private string GenerateAccessToken(User user)
        {
            // This should be implemented in a shared service
            // For now, it's just a placeholder
            return Guid.NewGuid().ToString();
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}