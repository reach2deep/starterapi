using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Global;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Core.Interfaces.Services;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;
using starterkit.Application.Persistence;
using starterkit.Core.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace starterkit.Application.Modules.Tenant.UserManagement.Services
{
    /// <summary>
    /// Service implementation for managing users in tenant context
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRootDbContext _rootContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IValidator<CreateUserRequest> _createValidator;
        private readonly IValidator<UpdateUserRequest> _updateValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _tenantId;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRootDbContext rootContext,
            IMapper mapper,
            ILogger<UserService> logger,
            IPasswordHashService passwordHashService,
            IValidator<CreateUserRequest> createValidator,
            IValidator<UpdateUserRequest> updateValidator,
            IValidator<ChangePasswordRequest> changePasswordValidator,
            IValidator<ResetPasswordRequest> resetPasswordValidator,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _rootContext = rootContext;
            _mapper = mapper;
            _logger = logger;
            _passwordHashService = passwordHashService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _changePasswordValidator = changePasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
            _httpContextAccessor = httpContextAccessor;
            _tenantId = _httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
        }

        public async Task<ApiResponse<UserResponse>> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<UserResponse>.CreateError("User not found", "NOT_FOUND");

            var response = _mapper.Map<UserResponse>(user);
            return ApiResponse<UserResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<IEnumerable<UserResponse>>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<UserResponse>>(users);
            return ApiResponse<IEnumerable<UserResponse>>.CreateSuccess(response);
        }

        public async Task<ApiResponse<PagedResponse<UserResponse>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var (users, totalCount) = await _userRepository.GetPagedAsync(pageNumber, pageSize);
            var mappedUsers = _mapper.Map<IEnumerable<UserResponse>>(users);
            var response = new PagedResponse<UserResponse>(mappedUsers, totalCount, pageNumber, pageSize);
            return ApiResponse<PagedResponse<UserResponse>>.CreateSuccess(response);
        }

        public async Task<ApiResponse<UserResponse>> CreateAsync(CreateUserRequest request)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UserResponse>.CreateError(validationResult.Errors.First().ErrorMessage, "VALIDATION_ERROR");

                // Get current tenant
                var tenant = await _rootContext.Tenants
                    .FirstOrDefaultAsync(t => t.DatabaseName == _tenantId);
                if (tenant == null)
                    return ApiResponse<UserResponse>.CreateError("Tenant not found", "TENANT_NOT_FOUND");

                // Check if email exists in current tenant - Use case-insensitive comparison
                var normalizedEmail = request.Email.ToLower();
                if (await _userRepository.ExistsByEmailAsync(normalizedEmail))
                    return ApiResponse<UserResponse>.CreateError($"User with email '{request.Email}' already exists in this tenant", "DUPLICATE_EMAIL");

                // Check if email exists in global database - Use case-insensitive comparison
                var existingGlobalUser = await _rootContext.GlobalUsers
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail && u.IsActive);

                // Check if user already exists in this tenant
                if (existingGlobalUser != null)
                {
                    var existingMapping = await _rootContext.TenantUserMappings
                        .FirstOrDefaultAsync(m => m.TenantId == tenant.Id && m.UserId == existingGlobalUser.Id);
                    
                    if (existingMapping != null)
                    {
                        return ApiResponse<UserResponse>.CreateError($"User with email '{request.Email}' already has access to this tenant", "USER_EXISTS_IN_TENANT");
                    }
                }

                Guid userId;
                bool isNewGlobalUser = false;

                if (existingGlobalUser != null)
                {
                    // Use existing global user's ID
                    userId = existingGlobalUser.Id;
                    _logger.LogInformation("Using existing global user {UserId} for email {Email} in tenant {TenantId}", 
                        userId, normalizedEmail, tenant.Id);
                }
                else
                {
                    // Create new global user
                    var globalUser = new GlobalUser
                    {
                        Email = normalizedEmail,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PasswordHash = _passwordHashService.HashPassword(request.Password),
                        UserType = UserType.User,
                        Status = UserStatus.Active,
                        CreatedBy = GetCurrentUserId()
                    };

                    await _rootContext.GlobalUsers.AddAsync(globalUser);
                    await _rootContext.SaveChangesAsync();
                    userId = globalUser.Id;
                    isNewGlobalUser = true;
                    _logger.LogInformation("Created new global user {UserId} for email {Email}", userId, normalizedEmail);
                }

                try
                {
                    // Create tenant user with same ID
                    var user = new User
                    {
                        Id = userId, // Use the same ID as global user
                        Email = normalizedEmail,
                        PasswordHash = _passwordHashService.HashPassword(request.Password),
                        FullName = $"{request.FirstName} {request.LastName}",
                        Status = UserStatus.Active,
                        CreatedBy = GetCurrentUserId()
                    };

                    // Create user profile
                    var profile = new UserProfile
                    {
                        Id = Guid.NewGuid(), // Generate new ID for profile
                        UserId = userId, // Set the UserId to match the user
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        CreatedBy = GetCurrentUserId()
                    };

                    // Set address if provided
                    if (request.Address != null)
                    {
                        profile.Address = new Address
                        {
                            Id = Guid.NewGuid(), // Generate new ID for address
                            StreetAddress = request.Address.StreetAddress,
                            City = request.Address.City,
                            Country = request.Address.Country,
                            PostalCode = request.Address.PostalCode,
                            State = request.Address.State,
                            CreatedBy = GetCurrentUserId()
                        };
                    }

                    user.Profile = profile;

                    // Create user in tenant database
                    user = await _userRepository.CreateAsync(user);
                    _logger.LogInformation("Created tenant user {UserId} in tenant {TenantId}", userId, tenant.Id);

                    // Assign roles in tenant database
                    if (request.RoleIds != null && request.RoleIds.Any())
                    {
                        var roles = new List<Role>();
                        foreach (var roleId in request.RoleIds)
                        {
                            var role = await _roleRepository.GetByIdAsync(roleId);
                            if (role == null) continue;

                            roles.Add(role);
                            var userRole = new UserRole
                            {
                                Id = Guid.NewGuid(), // Generate new ID for user role
                                UserId = user.Id,
                                RoleId = roleId,
                                CreatedBy = GetCurrentUserId()
                            };
                            await _roleRepository.CreateUserRoleAsync(userRole);
                            _logger.LogInformation("Assigned role {RoleId} to user {UserId} in tenant {TenantId}", 
                                roleId, userId, tenant.Id);
                        }

                        // Create tenant user mapping with the first role
                        if (roles.Any())
                        {
                            var mapping = new TenantUserMapping
                            {
                                TenantId = tenant.Id,
                                UserId = user.Id,
                                Role = roles.First().Name,
                                CreatedBy = GetCurrentUserId()
                            };
                            await _rootContext.TenantUserMappings.AddAsync(mapping);
                            await _rootContext.SaveChangesAsync();
                            _logger.LogInformation("Created tenant user mapping for user {UserId} in tenant {TenantId} with role {Role}", 
                                userId, tenant.Id, roles.First().Name);
                        }
                    }

                    // Refresh user to include roles
                    user = await _userRepository.GetByIdAsync(user.Id);
                    var response = _mapper.Map<UserResponse>(user);
                    return ApiResponse<UserResponse>.CreateSuccess(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating tenant user {UserId} in tenant {TenantId}. Rolling back...", userId, tenant.Id);
                    
                    // Try to clean up the global user if we created one
                    if (isNewGlobalUser)
                    {
                        try
                        {
                            var globalUser = await _rootContext.GlobalUsers.FindAsync(userId);
                            if (globalUser != null)
                            {
                                _rootContext.GlobalUsers.Remove(globalUser);
                                await _rootContext.SaveChangesAsync();
                                _logger.LogInformation("Rolled back global user creation for {UserId}", userId);
                            }
                        }
                        catch (Exception cleanupEx)
                        {
                            _logger.LogError(cleanupEx, "Error cleaning up global user {UserId} after tenant user creation failure", userId);
                        }
                    }
                    
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with email {Email} in tenant {TenantId}", 
                    request.Email, _tenantId);
                return ApiResponse<UserResponse>.CreateError($"Error creating user: {ex.Message}", "INTERNAL_ERROR");
            }
        }

        public async Task<ApiResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(request);
                if (!validationResult.IsValid)
                    return ApiResponse<UserResponse>.CreateError(validationResult.Errors.First().ErrorMessage, "VALIDATION_ERROR");

                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return ApiResponse<UserResponse>.CreateError("User not found", "NOT_FOUND");

                if (await _userRepository.ExistsByEmailAsync(request.Email, id))
                    return ApiResponse<UserResponse>.CreateError($"User with email '{request.Email}' already exists", "DUPLICATE_EMAIL");

                // Update global user
                var globalUser = await _rootContext.GlobalUsers.FindAsync(id);
                if (globalUser != null)
                {
                    globalUser.Email = request.Email;
                    globalUser.FirstName = request.FirstName;
                    globalUser.LastName = request.LastName;
                    globalUser.Status = request.Status;
                    globalUser.UpdatedBy = GetCurrentUserId();
                    globalUser.UpdatedAt = DateTime.UtcNow;
                }

                // Update tenant user
                user.Email = request.Email;
                user.FullName = $"{request.FirstName} {request.LastName}";
                user.Status = request.Status;
                user.UpdatedBy = GetCurrentUserId();
                user.UpdatedAt = DateTime.UtcNow;

                if (user.Profile == null)
                {
                    user.Profile = new UserProfile
                    {
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        CreatedBy = GetCurrentUserId()
                    };
                }
                else
                {
                    user.Profile.FirstName = request.FirstName;
                    user.Profile.LastName = request.LastName;
                    user.Profile.UpdatedBy = GetCurrentUserId();
                    user.Profile.UpdatedAt = DateTime.UtcNow;
                }

                user.Profile.Address = _mapper.Map<Address>(request.Address);

                // Update roles
                if (request.RoleIds != null)
                {
                    // Get current roles
                    var currentRoles = await _roleRepository.GetByUserIdAsync(id);
                    var currentRoleIds = currentRoles.Select(r => r.Id);

                    // Remove roles that are not in the new list
                    var rolesToRemove = currentRoleIds.Except(request.RoleIds);
                    if (rolesToRemove.Any())
                    {
                        await _roleRepository.RemoveUserRolesAsync(id, rolesToRemove);
                    }

                    // Add new roles
                    var rolesToAdd = request.RoleIds.Except(currentRoleIds);
                    foreach (var roleId in rolesToAdd)
                    {
                        var role = await _roleRepository.GetByIdAsync(roleId);
                        if (role == null) continue;

                        var userRole = new UserRole
                        {
                            UserId = user.Id,
                            RoleId = roleId,
                            CreatedBy = GetCurrentUserId()
                        };
                        await _roleRepository.CreateUserRoleAsync(userRole);
                    }

                    // Update TenantUserMapping with the highest role
                    var tenant = await _rootContext.Tenants
                        .FirstOrDefaultAsync(t => t.DatabaseName == _tenantId);
                    if (tenant != null)
                    {
                        var highestRole = await _roleRepository.GetByIdAsync(request.RoleIds.First());
                        var mapping = await _rootContext.TenantUserMappings
                            .FirstOrDefaultAsync(m => m.TenantId == tenant.Id && m.UserId == id);

                        if (mapping != null && highestRole != null)
                        {
                            mapping.Role = highestRole.Name;
                            mapping.UpdatedBy = GetCurrentUserId();
                            mapping.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                await _rootContext.SaveChangesAsync();
                user = await _userRepository.UpdateAsync(user);

                // Refresh user to include updated roles
                user = await _userRepository.GetByIdAsync(user.Id);
                var response = _mapper.Map<UserResponse>(user);
                return ApiResponse<UserResponse>.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return ApiResponse<UserResponse>.CreateError("Error updating user", "INTERNAL_ERROR");
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<bool>.CreateError("User not found", "NOT_FOUND");

            await _userRepository.DeleteAsync(id);
            return ApiResponse<bool>.CreateSuccess(true);
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return ApiResponse<bool>.CreateError(validationResult.Errors.First().ErrorMessage, "VALIDATION_ERROR");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.CreateError("User not found", "NOT_FOUND");

            if (!_passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                return ApiResponse<bool>.CreateError("Current password is incorrect", "INVALID_PASSWORD");

            if (request.NewPassword != request.ConfirmPassword)
                return ApiResponse<bool>.CreateError("New password and confirm password do not match", "PASSWORD_MISMATCH");

            var newPasswordHash = _passwordHashService.HashPassword(request.NewPassword);
            await _userRepository.UpdatePasswordAsync(userId, newPasswordHash);

            return ApiResponse<bool>.CreateSuccess(true);
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(Guid userId, ResetPasswordRequest request)
        {
            var validationResult = await _resetPasswordValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return ApiResponse<bool>.CreateError(validationResult.Errors.First().ErrorMessage, "VALIDATION_ERROR");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return ApiResponse<bool>.CreateError("User not found", "NOT_FOUND");

            var newPasswordHash = _passwordHashService.HashPassword(request.NewPassword);
            await _userRepository.UpdatePasswordAsync(userId, newPasswordHash);

            return ApiResponse<bool>.CreateSuccess(true);
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return userIdString != null ? Guid.Parse(userIdString) : Guid.Empty;
        }
    }
} 