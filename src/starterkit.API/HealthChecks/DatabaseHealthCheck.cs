using Microsoft.Extensions.Diagnostics.HealthChecks;
using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Persistence.TenantDb;
using Microsoft.EntityFrameworkCore;

namespace starterkit.API.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly IDbContextFactory<RootDbContext> _rootDbContextFactory;
    private readonly IDbContextFactory<TenantDbContext> _tenantDbContextFactory;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(
        IDbContextFactory<RootDbContext> rootDbContextFactory,
        IDbContextFactory<TenantDbContext> tenantDbContextFactory,
        ILogger<DatabaseHealthCheck> logger)
    {
        _rootDbContextFactory = rootDbContextFactory;
        _tenantDbContextFactory = tenantDbContextFactory;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var isHealthy = true;
        var data = new Dictionary<string, object>();

        try
        {
            // Check Root DB
            using (var rootDb = await _rootDbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                var canConnect = await rootDb.Database.CanConnectAsync(cancellationToken);
                data.Add("RootDatabase", canConnect ? "Healthy" : "Unhealthy");
                isHealthy &= canConnect;
            }

            // Check Tenant DB (using default tenant connection)
            using (var tenantDb = await _tenantDbContextFactory.CreateDbContextAsync(cancellationToken))
            {
                var canConnect = await tenantDb.Database.CanConnectAsync(cancellationToken);
                data.Add("TenantDatabase", canConnect ? "Healthy" : "Unhealthy");
                isHealthy &= canConnect;
            }

            return isHealthy
                ? HealthCheckResult.Healthy("All database connections are healthy", data)
                : HealthCheckResult.Unhealthy("One or more database connections are unhealthy", null, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return HealthCheckResult.Unhealthy("Database health check failed", ex, data);
        }
    }
} 