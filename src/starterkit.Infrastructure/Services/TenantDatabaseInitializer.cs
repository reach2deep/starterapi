using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Infrastructure.Data;
using starterkit.Infrastructure.Data.TenantDb;

namespace starterkit.Infrastructure.Services
{
    public interface ITenantDatabaseInitializer
    {
        Task InitializeTenantDatabaseAsync(string tenantId);
    }

    public class TenantDatabaseInitializer : ITenantDatabaseInitializer
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public TenantDatabaseInitializer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task InitializeTenantDatabaseAsync(string tenantId)
        {
            using var scope = _scopeFactory.CreateScope();
            var tenantDbFactory = scope.ServiceProvider.GetRequiredService<Func<string, TenantDbContext>>();
            var tenantContext = tenantDbFactory(tenantId);

            // Create database if it doesn't exist and apply migrations
            await tenantContext.Database.MigrateAsync();

            // Get the seeder and run it
            var seeder = ActivatorUtilities.CreateInstance<TenantDbSeeder>(
                scope.ServiceProvider,
                tenantContext,
                tenantId
            );

            await seeder.SeedAsync();
        }
    }
} 