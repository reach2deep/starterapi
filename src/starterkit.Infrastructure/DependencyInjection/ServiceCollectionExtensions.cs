using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;
using starterkit.Infrastructure.Repositories.Tenant;
using starterkit.Infrastructure.Services;

namespace starterkit.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            // Add repositories
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
} 