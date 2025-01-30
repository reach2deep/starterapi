using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Core.Modules.Global;
using starterkit.Application.Persistence;
using BCrypt.Net;
using starterkit.Core.Enums;

namespace starterkit.Infrastructure.Services
{
    public class TenantDatabaseInitializer : ITenantDatabaseInitializer
    {
        private readonly IDbContextFactory<DbContext> _dbContextFactory;
        private readonly IRootDbContext _rootContext;

        public TenantDatabaseInitializer(
            IDbContextFactory<DbContext> dbContextFactory,
            IRootDbContext rootContext)
        {
            _dbContextFactory = dbContextFactory;
            _rootContext = rootContext;
        }

        public async Task InitializeTenantDatabaseAsync(Tenant tenant)
        {
            // Create a new DbContext for the tenant
            using var context = _dbContextFactory.CreateDbContext();

            // Set the connection string
            context.Database.SetConnectionString(tenant.ConnectionString);

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Apply migrations
            await context.Database.MigrateAsync();

            // Seed initial data if needed
            await SeedInitialDataAsync(context);
        }

        public async Task InitializeTenantDatabaseAsync(string databaseName)
        {
            // Find the tenant by database name
            var tenant = await _rootContext.Tenants
                .FirstOrDefaultAsync(t => t.DatabaseName == databaseName);

            if (tenant == null)
            {
                throw new InvalidOperationException($"Tenant with database name {databaseName} not found");
            }

            await InitializeTenantDatabaseAsync(tenant);
        }

        private async Task SeedInitialDataAsync(DbContext context)
        {
            var tenant = await _rootContext.Tenants
                .FirstOrDefaultAsync(t => t.ConnectionString == context.Database.GetConnectionString());

            if (tenant == null)
            {
                throw new InvalidOperationException("Tenant not found for the given connection string");
            }

            // First create the global user for authentication
            var adminEmail = $"admin@{tenant.Name.ToLower()}.com";
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

            var globalAdmin = new GlobalUser
            {
                Id = Guid.NewGuid(),
                Email = adminEmail,
                FirstName = tenant.Name,
                LastName = "Admin",
                PasswordHash = passwordHash,
                UserType = UserType.User,
                Status = UserStatus.Active,
                CreatedBy = tenant.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            // Check if global user already exists
            var existingGlobalAdmin = await _rootContext.GlobalUsers
                .FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (existingGlobalAdmin == null)
            {
                await _rootContext.GlobalUsers.AddAsync(globalAdmin);
                await _rootContext.SaveChangesAsync();
            }
            else
            {
                globalAdmin = existingGlobalAdmin;
            }

            // Create tenant admin in tenant's Users table
            var tenantAdmin = new Core.Modules.Tenant.User
            {
                Id = globalAdmin.Id, // Use same ID as global user for consistency
                Email = adminEmail,
                FullName = $"{tenant.Name} Admin",
                PasswordHash = passwordHash,
                Status = UserStatus.Active,
                CreatedBy = tenant.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            // Check if user already exists in tenant database
            var existingTenantAdmin = await context.Set<Core.Modules.Tenant.User>()
                .FirstOrDefaultAsync(u => u.Id == globalAdmin.Id || u.Email == adminEmail);

            if (existingTenantAdmin == null)
            {
                context.Set<Core.Modules.Tenant.User>().Add(tenantAdmin);

                // Create user profile for the admin
                var userProfile = new Core.Modules.Tenant.UserProfile
                {
                    UserId = tenantAdmin.Id,
                    CreatedBy = tenant.CreatedBy,
                    CreatedAt = DateTime.UtcNow
                };

                context.Set<Core.Modules.Tenant.UserProfile>().Add(userProfile);
                await context.SaveChangesAsync();
            }

            // Create tenant user mapping in root database
            var tenantUserMapping = new TenantUserMapping
            {
                TenantId = tenant.Id,
                UserId = globalAdmin.Id,
                Role = "TenantAdmin",
                IsActive = true,
                CreatedBy = tenant.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            // Check if mapping already exists
            var existingMapping = await _rootContext.Set<TenantUserMapping>()
                .FirstOrDefaultAsync(m => m.TenantId == tenant.Id && m.UserId == globalAdmin.Id);

            if (existingMapping == null)
            {
                await _rootContext.Set<TenantUserMapping>().AddAsync(tenantUserMapping);
                await _rootContext.SaveChangesAsync();
            }
        }
    }
}