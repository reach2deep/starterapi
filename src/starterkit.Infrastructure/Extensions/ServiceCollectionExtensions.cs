using Microsoft.Extensions.DependencyInjection;
using starterkit.Core.Interfaces.Repositories.Tenant;
using starterkit.Core.Interfaces.Services.Tenant;
using starterkit.Infrastructure.Data.TenantDb.Repositories;
using starterkit.Application.Services.Tenant;

namespace starterkit.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantRepositories(this IServiceCollection services)
        {
            // Add tenant repositories
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Add tenant services
            services.AddScoped<IUserProfileService, UserProfileService>();

            return services;
        }
    }
} 