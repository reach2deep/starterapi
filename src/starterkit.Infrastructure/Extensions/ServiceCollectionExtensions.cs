using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.AspNetCore.Http;
using starterkit.Application.Persistence;
using starterkit.Infrastructure.Stores;
using starterkit.Infrastructure.Data.TenantDb;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Infrastructure.Data.TenantDb.Repositories;
using starterkit.Application.Modules.Tenant.UserManagement.Services;
using starterkit.Infrastructure.Services;
using FluentValidation;
using System.Reflection;
using n.Modules.Global.Auth.Services;
using starterkit.Application.Modules.Global.Auth.Mappings;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Application.Modules.Global.TenantManagement.Validators;
using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Persistence.TenantDb;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Logging;
using starterkit.Infrastructure.Data;

using starterkit.Application.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Application.Modules.Tenant.PermissionManagement.Services;
using starterkit.Infrastructure.Persistence.TenantDb.Repositories;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;
using starterkit.Core.Modules.Tenant.PermissionManagement.Interfaces;
using starterkit.Core.Modules.Global.TenantManagement.Interfaces.Repositories;
using starterkit.Infrastructure.Repositories.Global;
using starterkit.Core.Interfaces.Services;

namespace starterkit.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register AutoMapper from Application assembly
            services.AddAutoMapper(typeof(GlobalMappingProfile).Assembly);

            // Register FluentValidation from both assemblies
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly()); // Infrastructure validators
            services.AddValidatorsFromAssembly(typeof(CreateTenantRequestValidator).Assembly); // Application validators

            // Register Application Services
            services.Scan(scan => scan
                .FromAssemblyOf<GlobalAuthService>()
                .AddClasses(classes => classes.Where(type =>
                    type.Name.EndsWith("Service") &&
                    !type.IsAbstract &&
                    !type.IsInterface))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            // Register root database context
            services.AddDbContext<RootDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("RootConnection")));
            services.AddScoped<IRootDbContext>(sp => sp.GetRequiredService<RootDbContext>());

            // Add tenant store (needs to be before tenant context registration)
            services.AddScoped<ITenantStore, CachedTenantStore>();

            // Register tenant database context options
            services.AddSingleton(new DbContextOptionsBuilder<TenantDbContext>()
                .UseSqlServer(configuration.GetConnectionString("TenantConnection"))
                .Options);

            // Register DbContext factory for tenant initialization
            services.AddScoped<IDbContextFactory<DbContext>>(sp =>
            {
                var options = sp.GetRequiredService<DbContextOptions<TenantDbContext>>();
                return new TenantDbContextFactory(options);
            });

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
            services.AddScoped<IPermissionRepository, PermissionRepository>();

            // Add tenant services
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IPermissionService, PermissionService>();

            // Add tenant database initializer
            services.AddScoped<ITenantDatabaseInitializer, TenantDatabaseInitializer>();

            // Register tenant DbContext factory
            services.AddScoped<Func<string, IDataSeeder>>(sp => tenantId =>
            {
                var context = sp.GetRequiredService<Func<string, TenantDbContext>>()(tenantId);
                var rootContext = sp.GetRequiredService<RootDbContext>();
                var passwordHashService = sp.GetRequiredService<IPasswordHashService>();
                var logger = sp.GetRequiredService<ILogger<TenantDbSeeder>>();
                return new TenantDbSeeder(context, rootContext, passwordHashService, logger, tenantId);
            });

            // Register repositories
            services.AddScoped<ITenantRepository, TenantRepository>();

            // Register core services
            services.AddScoped<IPasswordHashService, PasswordHashService>();

            return services;
        }
    }

    internal class TenantDbContextFactory : IDbContextFactory<DbContext>, IDesignTimeDbContextFactory<TenantDbContext>
    {
        private readonly DbContextOptions<TenantDbContext> _options;

        // Constructor for runtime usage
        public TenantDbContextFactory(DbContextOptions<TenantDbContext> options)
        {
            _options = options;
        }

        // Parameterless constructor for design-time usage
        public TenantDbContextFactory()
        {
        }

        public DbContext CreateDbContext()
        {
            // Use a temporary tenant ID for initialization purposes
            return new TenantDbContext(_options, "temp");
        }

        public TenantDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=StarterKit_Tenant;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            
            return new TenantDbContext(optionsBuilder.Options, "design-time-tenant");
        }
    }
}