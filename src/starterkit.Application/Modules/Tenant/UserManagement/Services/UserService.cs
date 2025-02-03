using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Tenant;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Core.Interfaces.Services;

namespace starterkit.Application.Modules.Tenant.UserManagement.Services
{
    /// <summary>
    /// Service implementation for managing users in tenant context
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IValidator<CreateUserRequest> _createValidator;
        private readonly IValidator<UpdateUserRequest> _updateValidator;
        private readonly IValidator<ChangePasswordRequest> _changePasswordValidator;
        private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;

        public UserService(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<UserService> logger,
            IPasswordHashService passwordHashService,
            IValidator<CreateUserRequest> createValidator,
            IValidator<UpdateUserRequest> updateValidator,
            IValidator<ChangePasswordRequest> changePasswordValidator,
            IValidator<ResetPasswordRequest> resetPasswordValidator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _passwordHashService = passwordHashService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _changePasswordValidator = changePasswordValidator;
            _resetPasswordValidator = resetPasswordValidator;
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
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return ApiResponse<UserResponse>.CreateError(validationResult.Errors.First().ErrorMessage, "VALIDATION_ERROR");

            if (await _userRepository.ExistsByEmailAsync(request.Email))
                return ApiResponse<UserResponse>.CreateError($"User with email '{request.Email}' already exists", "DUPLICATE_EMAIL");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = _passwordHashService.HashPassword(request.Password),
                FullName = $"{request.FirstName} {request.LastName}",
                Status = Core.Enums.UserStatus.Active,
                Profile = new UserProfile
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Address = _mapper.Map<Address>(request.Address)
                }
            };

            user = await _userRepository.CreateAsync(user);
            var response = _mapper.Map<UserResponse>(user);
            return ApiResponse<UserResponse>.CreateSuccess(response);
        }

        public async Task<ApiResponse<UserResponse>> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return ApiResponse<UserResponse>.CreateError(validationResult.Errors.First().ErrorMessage, "VALIDATION_ERROR");

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return ApiResponse<UserResponse>.CreateError("User not found", "NOT_FOUND");

            if (await _userRepository.ExistsByEmailAsync(request.Email, id))
                return ApiResponse<UserResponse>.CreateError($"User with email '{request.Email}' already exists", "DUPLICATE_EMAIL");

            user.Email = request.Email;
            user.FullName = $"{request.FirstName} {request.LastName}";
            user.Status = request.Status;
            user.Profile.FirstName = request.FirstName;
            user.Profile.LastName = request.LastName;
            user.Profile.Address = _mapper.Map<Address>(request.Address);

            user = await _userRepository.UpdateAsync(user);
            var response = _mapper.Map<UserResponse>(user);
            return ApiResponse<UserResponse>.CreateSuccess(response);
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
    }
} 