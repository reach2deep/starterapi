using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using starterkit.API.HealthChecks;

namespace starterkit.API.DependencyInjection;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>(
                name: "database_health_check",
                tags: new[] { "ready", "db" }
            );

        return services;
    }

    public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
    {
        // Basic liveness check - just confirms the API is running
        app.UseHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false, // No checks, just returns 200 OK
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        });

        // Readiness check - includes all health checks
        app.UseHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = _ => true, // Run all checks
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        });

        // Detailed health report - can be used for monitoring
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        });

        return app;
    }
} 