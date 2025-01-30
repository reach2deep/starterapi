using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Core.Modules.Global;
using starterkit.Application.Persistence;

namespace starterkit.Infrastructure.Services
{
    public class TenantDatabaseInitializer : ITenantDatabaseInitializer
    {
        private readonly IDbContextFactory<DbContext> _dbContextFactory;
        private readonly IRootDbContext _rootContext;

        public TenantDatabaseInitializer(
            IDbContextFactory<DbContext> dbContextFactory,
            IRootDbContext rootContext)
        {
            _dbContextFactory = dbContextFactory;
            _rootContext = rootContext;
        }

        public async Task InitializeTenantDatabaseAsync(Tenant tenant)
        {
            // Create a new DbContext for the tenant
            using var context = _dbContextFactory.CreateDbContext();

            // Set the connection string
            context.Database.SetConnectionString(tenant.ConnectionString);

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed initial data if needed
            await SeedInitialDataAsync(context);
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

        private async Task SeedInitialDataAsync(DbContext context)
        {
            // Add your seeding logic here
            // Example: Add default roles, settings, etc.
            await context.SaveChangesAsync();
        }
    }
}