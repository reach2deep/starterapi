using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using n.Modules.Global.Auth.Services;
using starterkit.API.Middlewares;
using starterkit.API.DependencyInjection;

using starterkit.Application.Modules.Global.Auth.Interfaces;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Services;
using starterkit.Core.Enums;
using starterkit.Infrastructure.Data;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Data.TenantDb;


using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Infrastructure.Services;
using starterkit.Infrastructure.Stores;
using System.Text;
using starterkit.Core.Modules.Global.Auth.Interfaces.Repositories;
using starterkit.Core.Modules.Tenant.UserManagement.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .SetIsOriginAllowed(_ => true)  // Allow any origin
            .AllowAnyMethod()               // Allow all HTTP methods
            .AllowAnyHeader()               // Allow all headers
            .AllowCredentials();            // Allow credentials (important for auth!)
    });
});

// Add controllers and configure JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

// Register all services using our new structure
builder.Services.AddApplicationStack(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();
}

app.UseHttpsRedirection();

// Add health checks before exception handling
app.UseCustomHealthChecks();

// Add global exception handling
app.UseGlobalExceptionHandling();

// Ensure correct middleware order
app.UseRouting();
app.UseCors(); // Must come after UseRouting but before Authentication
app.UseAuthentication();
app.UseAuthorization();
app.UseTenantMiddleware();

app.MapControllers();

// Initialize databases
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var rootContext = services.GetRequiredService<RootDbContext>();
    var tenantInitializer = services.GetRequiredService<ITenantDatabaseInitializer>();
    var seeder = services.GetRequiredService<IDataSeeder>();

    // Ensure root database is created and migrated
    await rootContext.Database.MigrateAsync();

    // Seed root database
    await seeder.SeedAsync();

    // Initialize databases for all active tenants
    var activeTenants = await rootContext.Tenants
        .Where(t => t.IsActive && t.Status == TenantStatus.Active)
        .ToListAsync();

    foreach (var tenant in activeTenants)
    {
        try
        {
            await tenantInitializer.InitializeTenantDatabaseAsync(tenant);
        }
        catch (Exception ex)
        {
            // Log the error but continue with other tenants
            Console.WriteLine($"Failed to initialize database for tenant {tenant.Name}: {ex.Message}");
        }
    }
}

app.Run();

