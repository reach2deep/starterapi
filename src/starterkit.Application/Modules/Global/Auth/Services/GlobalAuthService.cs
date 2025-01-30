using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using starterkit.Application.DTOs.Global.Auth;
using starterkit.Application.Interfaces.Services.Global;
using starterkit.Core.Entities.Global;
using starterkit.Core.Exceptions.Auth;
using starterkit.Core.Exceptions.Tenant;
using starterkit.Core.Interfaces.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace starterkit.Application.Services.Global
{
    public class GlobalAuthService : IGlobalAuthService
    {
        private readonly IRootDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GlobalAuthService> _logger;
        private readonly IMapper _mapper;

        public GlobalAuthService(
            IRootDbContext context,
            IConfiguration configuration,
            ILogger<GlobalAuthService> logger,
            IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GlobalLoginResponseDto> LoginAsync(GlobalLoginRequestDto request)
        {
            try
            {
                // Validate request
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (string.IsNullOrEmpty(request.Email)) throw new ArgumentException("Email is required", nameof(request));
                if (string.IsNullOrEmpty(request.Password)) throw new ArgumentException("Password is required", nameof(request));

                _logger.LogInformation("Attempting login for user: {Email}", request.Email);

                var user = await _context.GlobalUsers
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for user: {Email}", request.Email);
                    throw new InvalidCredentialsException();
                }

                // Get user's tenant mappings
                var tenantMappings = await _context.TenantUserMappings
                    .Include(t => t.Tenant)
                    .Where(t => t.UserId == user.Id && t.IsActive && t.Tenant.Status == Core.Enums.TenantStatus.Active)
                    .ToListAsync();

                if (!tenantMappings.Any())
                {
                    _logger.LogWarning("User {Email} has no active tenant mappings", request.Email);
                    throw new TenantAccessDeniedException("User has no active tenant access");
                }

                // Generate base token
                var baseToken = GenerateBaseToken(user);

                _logger.LogInformation("User {Email} successfully logged in", request.Email);

                // Map user to response DTO
                var response = _mapper.Map<GlobalLoginResponseDto>(user);
                response.BaseToken = baseToken;
                response.AvailableTenants = _mapper.Map<IEnumerable<TenantAccessDto>>(tenantMappings);

                return response;
            }
            catch (Exception ex) when (ex is not InvalidCredentialsException && 
                                     ex is not TenantAccessDeniedException && 
                                     ex is not ArgumentException)
            {
                _logger.LogError(ex, "An error occurred during login for user {Email}", request?.Email);
                throw;
            }
        }

        public async Task<TenantSelectionResponseDto> SelectTenantAsync(TenantSelectionRequestDto request)
        {
            try
            {
                // Validate request
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (string.IsNullOrEmpty(request.BaseToken)) throw new ArgumentException("Base token is required", nameof(request));
                if (request.TenantId == Guid.Empty) throw new ArgumentException("Valid tenant ID is required", nameof(request));

                _logger.LogInformation("Attempting tenant selection for tenant: {TenantId}", request.TenantId);

                var user = await ValidateBaseTokenAsync(request.BaseToken);
                if (user == null)
                {
                    _logger.LogWarning("Invalid base token used for tenant selection");
                    throw new InvalidTokenException("Invalid base token");
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
                    _logger.LogWarning("User {UserId} attempted to access unauthorized tenant {TenantId}", 
                        user.Id, request.TenantId);
                    throw new TenantAccessDeniedException(request.TenantId, user.Id);
                }

                // Generate JWT token
                var token = GenerateJwtToken(user, tenantMapping);
                var refreshToken = GenerateRefreshToken();

                _logger.LogInformation("Successfully generated tokens for user {UserId} in tenant {TenantId}", 
                    user.Id, request.TenantId);

                return new TenantSelectionResponseDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    ExpiresIn = 3600, // 1 hour
                    TokenType = "Bearer"
                };
            }
            catch (Exception ex) when (ex is not InvalidTokenException && 
                                     ex is not TenantAccessDeniedException && 
                                     ex is not ArgumentException)
            {
                _logger.LogError(ex, "An error occurred during tenant selection for tenant {TenantId}", request?.TenantId);
                throw;
            }
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