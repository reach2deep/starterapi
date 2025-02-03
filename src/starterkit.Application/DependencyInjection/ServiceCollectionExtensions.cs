using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.RoleManagement.Interfaces;
using starterkit.Application.Modules.Tenant.RoleManagement.Services;
using starterkit.Application.Modules.Tenant.RoleManagement.Validators;
using starterkit.Application.Modules.Tenant.UserManagement.DTOs;
using starterkit.Application.Modules.Tenant.UserManagement.Validators;

namespace starterkit.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add role management service
            services.AddScoped<IRoleService, RoleService>();

            

            // Add role validators
            services.AddScoped<IValidator<CreateRoleRequest>, CreateRoleRequestValidator>();
            services.AddScoped<IValidator<UpdateRoleRequest>, UpdateRoleRequestValidator>();
            services.AddScoped<IValidator<CopyRoleRequest>, CopyRoleRequestValidator>();

            // Add user validators
            services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
            services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();
            services.AddScoped<IValidator<ChangePasswordRequest>, ChangePasswordRequestValidator>();
            services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordRequestValidator>();

            // Register all validators in the assembly
            services.AddValidatorsFromAssemblyContaining<CreateRoleRequestValidator>();

            return services;
        }
    }
} 