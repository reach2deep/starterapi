using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;
using starterkit.Infrastructure.Repositories.Tenant;
using starterkit.Infrastructure.Services;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Services;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Core.Interfaces.Services;

namespace starterkit.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            // Add repositories
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Add services
           
            return services;
        }
    }
} 