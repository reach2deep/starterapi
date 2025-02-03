using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace starterkit.API.DependencyInjection;

/// <summary>
/// Main extension class for registering application services
/// </summary>
public static class ApplicationServiceExtensions
{
    /// <summary>
    /// Registers all application services in the correct order
    /// </summary>
    public static IServiceCollection AddApplicationStack(this IServiceCollection services, IConfiguration configuration)
    {
        // Register infrastructure first (database, logging, etc)
        services.AddInfrastructureStack(configuration);
        
        // Register tenant services (after infrastructure)
        services.AddTenantStack(configuration);
        
        // Register application services (depends on both infrastructure and tenant)
        services.AddModuleServices();
        
        // Register cross-cutting concerns
        services.AddCustomHealthChecks();
        services.AddCustomAuthentication(configuration);
        services.AddCustomSwagger();
        
        return services;
    }
} 