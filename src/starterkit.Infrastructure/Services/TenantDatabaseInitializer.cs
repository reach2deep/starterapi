using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Core.Modules.Global;
using starterkit.Application.Persistence;
using BCrypt.Net;
using starterkit.Core.Enums;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Data.TenantDb;


namespace starterkit.Infrastructure.Services
{
    public class TenantDatabaseInitializer : ITenantDatabaseInitializer
    {
        private readonly IDbContextFactory<DbContext> _dbContextFactory;
        private readonly IRootDbContext _rootContext;
        private readonly IPasswordHashService _passwordHashService;

        public TenantDatabaseInitializer(
            IDbContextFactory<DbContext> dbContextFactory,
            IRootDbContext rootContext,
            IPasswordHashService passwordHashService)
        {
            _dbContextFactory = dbContextFactory;
            _rootContext = rootContext;
            _passwordHashService = passwordHashService;
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

            // Create and run the tenant seeder
            var tenantSeeder = new TenantDbSeeder(
                (TenantDbContext)context,
                (RootDbContext)_rootContext,
                _passwordHashService,
                tenant.DatabaseName);

            await tenantSeeder.SeedAsync();
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