using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Core.Interfaces.Data;

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
            var tenantDbFactory = scope.ServiceProvider.GetRequiredService<Func<string, ITenantDbContext>>();
            var tenantContext = tenantDbFactory(tenantId);

            // For migrations, we need to cast to DbContext
            var dbContext = tenantContext as DbContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("Context must be a DbContext for migrations");
            }

            // Create database if it doesn't exist and apply migrations
            await dbContext.Database.MigrateAsync();
        }
    }
} 