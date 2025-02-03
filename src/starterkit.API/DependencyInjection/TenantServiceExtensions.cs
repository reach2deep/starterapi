using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Infrastructure.Services;
using starterkit.Application.Persistence;
using starterkit.Infrastructure.Stores;
using Microsoft.AspNetCore.Http;

namespace starterkit.API.DependencyInjection;

/// <summary>
/// Extensions for registering tenant-specific services
/// </summary>
public static class TenantServiceExtensions
{
    /// <summary>
    /// Registers tenant-specific services and database contexts
    /// </summary>
    public static IServiceCollection AddTenantStack(this IServiceCollection services, IConfiguration configuration)
    {
        // Add tenant store
        services.AddScoped<ITenantStore, CachedTenantStore>();
        services.AddScoped<ITenantResolver, TenantResolver>();

        // Register tenant database context factory
        services.AddSingleton(new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlServer(configuration.GetConnectionString("TenantConnection"))
            .Options);

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

        // Register scoped tenant context
        services.AddScoped(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var tenantId = httpContextAccessor.HttpContext?.Items["Tenant"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                throw new InvalidOperationException("Tenant ID not found in HTTP context");

            var factory = sp.GetRequiredService<Func<string, ITenantDbContext>>();
            return factory(tenantId);
        });

        return services;
    }
} 