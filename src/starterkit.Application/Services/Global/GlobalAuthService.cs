using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using starterkit.Application.DTOs.Global.Auth;
using starterkit.Core.Entities.Global;
using starterkit.Infrastructure.Data.RootDb;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace starterkit.Application.Services.Global
{
    public interface IGlobalAuthService
    {
        Task<GlobalLoginResponseDto> LoginAsync(GlobalLoginRequestDto request);
        Task<TenantSelectionResponseDto> SelectTenantAsync(TenantSelectionRequestDto request);
        Task<GlobalUser> ValidateBaseTokenAsync(string baseToken);
    }

    public class GlobalAuthService : IGlobalAuthService
    {
        private readonly RootDbContext _context;
        private readonly IConfiguration _configuration;

        public GlobalAuthService(RootDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<GlobalLoginResponseDto> LoginAsync(GlobalLoginRequestDto request)
        {
            var user = await _context.GlobalUsers
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Get user's tenant mappings
            var tenantMappings = await _context.Set<TenantUserMapping>()
                .Include(t => t.Tenant)
                .Where(t => t.UserId == user.Id && t.IsActive && t.Tenant.Status == Core.Enums.TenantStatus.Active)
                .ToListAsync();

            // Generate base token
            var baseToken = GenerateBaseToken(user);

            return new GlobalLoginResponseDto
            {
                BaseToken = baseToken,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                AvailableTenants = tenantMappings.Select(t => new TenantAccessDto
                {
                    TenantId = t.TenantId,
                    TenantName = t.Tenant.Name,
                    Role = t.Role
                })
            };
        }

        public async Task<TenantSelectionResponseDto> SelectTenantAsync(TenantSelectionRequestDto request)
        {
            var user = await ValidateBaseTokenAsync(request.BaseToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid base token");
            }

            // Verify tenant access
            var tenantMapping = await _context.Set<TenantUserMapping>()
                .Include(t => t.Tenant)
                .FirstOrDefaultAsync(t => 
                    t.TenantId == request.TenantId && 
                    t.UserId == user.Id && 
                    t.IsActive && 
                    t.Tenant.Status == Core.Enums.TenantStatus.Active);

            if (tenantMapping == null)
            {
                throw new UnauthorizedAccessException("User does not have access to this tenant");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user, tenantMapping);
            var refreshToken = GenerateRefreshToken();

            return new TenantSelectionResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600 // 1 hour
            };
        }

        public async Task<GlobalUser> ValidateBaseTokenAsync(string baseToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:BaseTokenSecret"]);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(baseToken, validationParameters, out _);
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return null;
                }

                return await _context.GlobalUsers.FindAsync(Guid.Parse(userId));
            }
            catch
            {
                return null;
            }
        }

        private string GenerateBaseToken(GlobalUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:BaseTokenSecret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.UserType.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateJwtToken(GlobalUser user, TenantUserMapping tenantMapping)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("tenant_id", tenantMapping.TenantId.ToString()),
                    new Claim(ClaimTypes.Role, tenantMapping.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            // Implement proper password verification here
            // This is just a placeholder
            return true;
        }
    }
} 