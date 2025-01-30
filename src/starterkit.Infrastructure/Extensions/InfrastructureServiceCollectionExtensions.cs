using Microsoft.Extensions.DependencyInjection;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.starterkit.Infrastructure.Services;

namespace starterkit.starterkit.Infrastructure.Extensions
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register infrastructure services
            services.AddScoped<ITenantDatabaseInitializer, TenantDatabaseInitializer>();

            return services;
        }
    }
} 