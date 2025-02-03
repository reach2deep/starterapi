using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Core.Interfaces.Services;
using starterkit.Infrastructure.Services;
using starterkit.Application.Persistence;
using starterkit.Infrastructure.Data;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Data.TenantDb;
using Microsoft.Extensions.Logging;

namespace starterkit.API.DependencyInjection;

/// <summary>
/// Extensions for registering infrastructure services
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registers core infrastructure services
    /// </summary>
    public static IServiceCollection AddInfrastructureStack(this IServiceCollection services, IConfiguration configuration)
    {
        // Register root database
        services.AddDbContext<RootDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("RootConnection")));
        services.AddScoped<IRootDbContext>(sp => sp.GetRequiredService<RootDbContext>());

        // Register tenant database options
        services.AddSingleton(new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlServer(configuration.GetConnectionString("TenantConnection"))
            .Options);

        // Register DbContext factory for tenant initialization
        services.AddScoped<IDbContextFactory<DbContext>>(sp =>
        {
            var options = sp.GetRequiredService<DbContextOptions<TenantDbContext>>();
            return new TenantDbContextFactory(options);
        });

        // Register tenant seeder factory
        services.AddScoped<Func<string, IDataSeeder>>(sp => tenantId =>
        {
            var context = sp.GetRequiredService<Func<string, TenantDbContext>>()(tenantId);
            var rootContext = sp.GetRequiredService<RootDbContext>();
            var passwordHashService = sp.GetRequiredService<IPasswordHashService>();
            var logger = sp.GetRequiredService<ILogger<TenantDbSeeder>>();
            return new TenantDbSeeder(context, rootContext, passwordHashService, logger, tenantId);
        });

        // Register core services
        services.AddScoped<IPasswordHashService, PasswordHashService>();
        services.AddScoped<ITenantDatabaseInitializer, TenantDatabaseInitializer>();
        services.AddScoped<IDataSeeder, RootDbSeeder>();
        
        // Register AutoMapper and FluentValidation from infrastructure assembly
        services.AddAutoMapper(typeof(RootDbContext).Assembly);
        services.AddValidatorsFromAssembly(typeof(RootDbContext).Assembly);

        return services;
    }
}

internal class TenantDbContextFactory : IDbContextFactory<DbContext>
{
    private readonly DbContextOptions<TenantDbContext> _options;

    public TenantDbContextFactory(DbContextOptions<TenantDbContext> options)
    {
        _options = options;
    }

    public DbContext CreateDbContext()
    {
        // Use a temporary tenant ID for initialization purposes
        return new TenantDbContext(_options, "temp");
    }
} 