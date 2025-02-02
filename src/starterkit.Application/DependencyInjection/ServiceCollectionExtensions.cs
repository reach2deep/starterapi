using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.RoleManagement.Interfaces;
using starterkit.Application.Modules.Tenant.RoleManagement.Services;
using starterkit.Application.Modules.Tenant.RoleManagement.Validators;

namespace starterkit.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Add role management service
            services.AddScoped<IRoleService, RoleService>();

            // Add validators
            services.AddScoped<IValidator<CreateRoleRequest>, CreateRoleRequestValidator>();
            services.AddScoped<IValidator<UpdateRoleRequest>, UpdateRoleRequestValidator>();
            services.AddScoped<IValidator<CopyRoleRequest>, CopyRoleRequestValidator>();

            // Register all validators in the assembly
            services.AddValidatorsFromAssemblyContaining<CreateRoleRequestValidator>();

            return services;
        }
    }
} 