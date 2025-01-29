using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Core.Interfaces.Data;
using starterkit.Core.Interfaces.Repositories.Tenant;
using starterkit.Core.Interfaces.Services.Tenant;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Data.TenantDb;
using starterkit.Infrastructure.Data.TenantDb.Repositories;
using starterkit.Application.Services.Tenant;
using starterkit.Infrastructure.MultiTenancy.Stores;
using starterkit.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace starterkit.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register root database context
            services.AddDbContext<RootDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("RootConnection")));
            services.AddScoped<IRootDbContext>(sp => sp.GetRequiredService<RootDbContext>());

            // Add tenant store (needs to be before tenant context registration)
            services.AddScoped<ITenantStore, CachedTenantStore>();

            // Register tenant DbContext factory
            services.AddScoped<Func<string, TenantDbContext>>(sp => tenantId =>
            {
                var tenantStore = sp.GetRequiredService<ITenantStore>();
                var tenant = tenantStore.GetTenantAsync(tenantId).Result;
                if (tenant == null)
                    throw new InvalidOperationException($"Tenant {tenantId} not found");

                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(tenant.ConnectionString);
                return new TenantDbContext(optionsBuilder.Options, tenantId);
            });

            // Register tenant context interface factory
            services.AddScoped<Func<string, ITenantDbContext>>(sp => tenantId =>
            {
                var factory = sp.GetRequiredService<Func<string, TenantDbContext>>();
                return factory(tenantId);
            });

            // Register scoped tenant context for repositories
            services.AddScoped(sp =>
            {
                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
                var tenantId = httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
                if (string.IsNullOrEmpty(tenantId))
                    throw new InvalidOperationException("Tenant ID not found in HTTP context");

                var factory = sp.GetRequiredService<Func<string, ITenantDbContext>>();
                return factory(tenantId);
            });

            // Add tenant repositories
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Add tenant services
            services.AddScoped<IUserProfileService, UserProfileService>();

            // Add tenant database initializer
            services.AddScoped<ITenantDatabaseInitializer, TenantDatabaseInitializer>();

            return services;
        }
    }
} 