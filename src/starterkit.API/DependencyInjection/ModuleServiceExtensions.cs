using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using n.Modules.Global.Auth.Services;
using starterkit.Application.Modules.Global.Auth.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Services;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Infrastructure.Repositories.Tenant;
using starterkit.Application.Modules.Tenant.RoleManagement.Services;
using starterkit.Application.Modules.Tenant.RoleManagement.Interfaces;
using starterkit.Infrastructure.Data.TenantDb.Repositories;
using starterkit.Application.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Application.Modules.Tenant.PermissionManagement.Services;
using starterkit.Core.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Infrastructure.Persistence.TenantDb.Repositories;

namespace starterkit.API.DependencyInjection;

/// <summary>
/// Extensions for registering module-specific services
/// </summary>
public static class ModuleServiceExtensions
{
    /// <summary>
    /// Registers all module services and their dependencies
    /// </summary>
    public static IServiceCollection AddModuleServices(this IServiceCollection services)
    {
        // Register auth module
        AddAuthModule(services);
        
        // Register user management module
        AddUserManagementModule(services);
        
        // Register role management module
        AddRoleManagementModule(services);
        
        // Register permission management module
        AddPermissionManagementModule(services);
        
        // Register AutoMapper and FluentValidation from application assembly
        services.AddAutoMapper(typeof(UserService).Assembly);
        services.AddValidatorsFromAssembly(typeof(UserService).Assembly);

        return services;
    }
    
    private static void AddAuthModule(IServiceCollection services)
    {
        services.AddScoped<IGlobalAuthService, GlobalAuthService>();
        services.AddScoped<ITenantAuthService, TenantAuthService>();
    }
    
    private static void AddUserManagementModule(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
    }
    
    private static void AddRoleManagementModule(IServiceCollection services)
    {
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleRepository, RoleRepository>();
    }
    
    private static void AddPermissionManagementModule(IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
    }
} 