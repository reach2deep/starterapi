using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Core.Interfaces.Data;
using starterkit.Infrastructure.Data.RootDb;
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
            
            // Get the root context to access tenant information
            var rootContext = scope.ServiceProvider.GetRequiredService<RootDbContext>();
            
            // Get the tenant context factory
            var tenantDbFactory = scope.ServiceProvider.GetRequiredService<Func<string, TenantDbContext>>();
            var tenantContext = tenantDbFactory(tenantId);

            // Create database if it doesn't exist and apply migrations
            await tenantContext.Database.MigrateAsync();

            // Create and run the seeder
            var passwordHashService = scope.ServiceProvider.GetRequiredService<IPasswordHashService>();
            var seeder = new TenantDbSeeder(tenantContext, rootContext, passwordHashService, tenantId);
            await seeder.SeedAsync();
        }
    }
} 