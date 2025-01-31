using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Core.Modules.Global;
using starterkit.Application.Persistence;
using starterkit.Infrastructure.Data;
using starterkit.Infrastructure.Data.TenantDb;

namespace starterkit.Infrastructure.Services
{
    public class TenantDatabaseInitializer : ITenantDatabaseInitializer
    {
        private readonly IDbContextFactory<DbContext> _dbContextFactory;
        private readonly IRootDbContext _rootContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TenantDatabaseInitializer> _logger;

        public TenantDatabaseInitializer(
            IDbContextFactory<DbContext> dbContextFactory,
            IRootDbContext rootContext,
            IServiceProvider serviceProvider,
            ILogger<TenantDatabaseInitializer> logger)
        {
            _dbContextFactory = dbContextFactory;
            _rootContext = rootContext;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task InitializeTenantDatabaseAsync(Tenant tenant)
        {
            try
            {
                _logger.LogInformation("Starting database initialization for tenant {TenantId}", tenant.DatabaseName);
                
                // Create a new DbContext for the tenant
                using var context = _dbContextFactory.CreateDbContext();

                // Set the connection string
                context.Database.SetConnectionString(tenant.ConnectionString);

                // Ensure database is created
                await context.Database.EnsureCreatedAsync();

                // Apply migrations
                await context.Database.MigrateAsync();

                // Get the seeder factory
                var seederFactory = _serviceProvider.GetRequiredService<Func<string, IDataSeeder>>();
                
                // Create and run the seeder with the tenant database name
                var seeder = seederFactory(tenant.DatabaseName);
                await seeder.SeedAsync();

                _logger.LogInformation("Successfully initialized database for tenant {TenantId}", tenant.DatabaseName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize database for tenant {TenantId}", tenant.DatabaseName);
                throw;
            }
        }

        public async Task InitializeTenantDatabaseAsync(string databaseName)
        {
            // Find the tenant by database name
            var tenant = await _rootContext.Tenants
                .FirstOrDefaultAsync(t => t.DatabaseName == databaseName);

            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant with database name {databaseName} not found");
            }

            await InitializeTenantDatabaseAsync(tenant);
        }
    }
}