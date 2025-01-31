using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using n.Modules.Global.Auth.Services;
using starterkit.API.Middlewares;
using starterkit.Application.Modules.Global.Auth.Interfaces;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Interfaces;
using starterkit.Application.Modules.Tenant.UserManagement.Services;
using starterkit.Core.Enums;
using starterkit.Infrastructure.Data;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Data.TenantDb;
using starterkit.Infrastructure.Extensions;
using starterkit.Infrastructure.Services;
using starterkit.Infrastructure.Stores;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Starter Kit API", Version = "v1" });

    // Configure schema IDs to use full type names
    c.CustomSchemaIds(type => type.FullName);

    // Add JWT Authentication
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

// Add memory cache
builder.Services.AddMemoryCache();

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

// Add tenant services
builder.Services.AddTenantServices(builder.Configuration);

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JWT:Secret"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Root Database
builder.Services.AddDbContext<RootDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RootConnection")));

// Configure Tenant Database (Scoped factory)
builder.Services.AddScoped<Func<string, TenantDbContext>>(serviceProvider => databaseName =>
{
    var tenant = serviceProvider.GetService<ITenantStore>()?.GetTenantAsync(databaseName).Result;
    if (tenant == null)
        throw new Exception($"Tenant with database name '{databaseName}' is not found");

    var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
    optionsBuilder.UseSqlServer(tenant.ConnectionString);
    return new TenantDbContext(optionsBuilder.Options, databaseName);
});

// Register services
builder.Services.AddScoped<ITenantStore, CachedTenantStore>();
builder.Services.AddScoped<IGlobalAuthService, GlobalAuthService>();
builder.Services.AddScoped<ITenantAuthService, TenantAuthService>();
builder.Services.AddScoped<IPasswordHashService, PasswordHashService>();
builder.Services.AddScoped<IDataSeeder, RootDbSeeder>();
builder.Services.AddScoped<ITenantDatabaseInitializer, TenantDatabaseInitializer>();
builder.Services.AddScoped<ITenantResolver, TenantResolver>();

// builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add global exception handling first
app.UseGlobalExceptionHandling();

// Add authentication & authorization first
app.UseAuthentication();
app.UseAuthorization();

// Then add tenant middleware
app.UseTenantMiddleware();

app.MapControllers();

// Initialize databases
using (var scope = app.Services.CreateScope())
{
   

    // Seed root database
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();

     var services = scope.ServiceProvider;
    var rootContext = services.GetRequiredService<RootDbContext>();

    var tenantInitializer = services.GetRequiredService<ITenantDatabaseInitializer>();

    // Ensure root database is created and migrated
    await rootContext.Database.MigrateAsync();

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

