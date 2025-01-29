using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using starterkit.API.Middlewares;
using starterkit.Application.Services.Global;
using starterkit.Application.Services.Tenant;
using starterkit.Infrastructure.Data;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Data.TenantDb;
using starterkit.Infrastructure.MultiTenancy.Stores;
using starterkit.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Starter Kit API", Version = "v1" });
    
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add tenant middleware before authentication
app.UseTenantMiddleware();

// Add authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed the root database
using (var scope = app.Services.CreateScope())
{
    // Seed root database
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();

    // Initialize tenant databases
    var tenantStore = scope.ServiceProvider.GetRequiredService<ITenantStore>();
    var databaseInitializer = scope.ServiceProvider.GetRequiredService<ITenantDatabaseInitializer>();

    // Get all tenants and initialize their databases
    var tenants = await tenantStore.GetAllTenantsAsync();
    foreach (var tenant in tenants)
    {
        await databaseInitializer.InitializeTenantDatabaseAsync(tenant.DatabaseName);
    }
}

app.Run();

