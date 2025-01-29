using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;
using starterkit.Core.Entities.Tenant;
using starterkit.Core.Enums;
using starterkit.Core.Interfaces.Data;
using starterkit.Infrastructure.Services;

namespace starterkit.Infrastructure.Data.TenantDb
{
    public class TenantDbSeeder : IDataSeeder
    {
        private readonly ITenantDbContext _context;
        private readonly IRootDbContext _rootContext;
        private readonly IPasswordHashService _passwordHashService;
        private readonly string _tenantId;

        public TenantDbSeeder(
            ITenantDbContext context, 
            IRootDbContext rootContext,
            IPasswordHashService passwordHashService,
            string tenantId)
        {
            _context = context;
            _rootContext = rootContext;
            _passwordHashService = passwordHashService;
            _tenantId = tenantId;
        }

        public async Task SeedAsync()
        {
            // For seeding, we need to cast to DbContext to use migrations
            var dbContext = _context as DbContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("Context must be a DbContext for migrations");
            }

            // Ensure database is migrated
            await dbContext.Database.MigrateAsync();

            if (!await _context.Users.AnyAsync())
            {
                // Get tenant info from root database
                var tenant = await _rootContext.Tenants.FirstOrDefaultAsync(t => t.DatabaseName == _tenantId);
                if (tenant == null)
                {
                    throw new Exception($"Tenant {_tenantId} not found in root database");
                }

                // Get root admin from root database
                var rootAdmin = await _rootContext.GlobalUsers.FirstOrDefaultAsync(u => u.UserType == UserType.RootAdmin);
                if (rootAdmin == null)
                {
                    throw new Exception("Root admin not found in root database");
                }

                // Create root admin in tenant database with same ID
                var rootAdminInTenant = new User
                {
                    Id = rootAdmin.Id, // Use the same ID as the global user
                    Email = rootAdmin.Email,
                    FullName = $"{rootAdmin.FirstName} {rootAdmin.LastName}",
                    PasswordHash = rootAdmin.PasswordHash,
                    Status = UserStatus.Active,
                    CreatedBy = rootAdmin.Id,
                    Profile = new UserProfile
                    {
                        CreatedBy = rootAdmin.Id
                    }
                };

                await _context.Users.AddAsync(rootAdminInTenant);
                await ((DbContext)_context).SaveChangesAsync();

                // First create the global user for tenant admin
                var globalTenantAdmin = new GlobalUser
                {
                    Email = $"admin@{tenant.Name.ToLower()}.com",
                    FirstName = tenant.Name,
                    LastName = "Admin",
                    PasswordHash = _passwordHashService.HashPassword("Admin@123"),
                    UserType = UserType.TenantAdmin,
                    Status = UserStatus.Active,
                    CreatedBy = rootAdmin.Id
                };

                await _rootContext.GlobalUsers.AddAsync(globalTenantAdmin);
                await ((DbContext)_rootContext).SaveChangesAsync();

                // Then create tenant admin in tenant database with same ID
                var tenantAdmin = new User
                {
                    Id = globalTenantAdmin.Id, // Use the same ID as the global user
                    Email = globalTenantAdmin.Email,
                    FullName = $"{globalTenantAdmin.FirstName} {globalTenantAdmin.LastName}",
                    PasswordHash = globalTenantAdmin.PasswordHash,
                    Status = UserStatus.Active,
                    CreatedBy = rootAdminInTenant.Id,
                    Profile = new UserProfile
                    {
                        CreatedBy = rootAdminInTenant.Id
                    }
                };

                await _context.Users.AddAsync(tenantAdmin);
                await ((DbContext)_context).SaveChangesAsync();

                // Create tenant-user mapping in root database
                var mapping = new TenantUserMapping
                {
                    TenantId = tenant.Id,
                    UserId = globalTenantAdmin.Id,
                    Role = "Admin",
                    CreatedBy = rootAdmin.Id
                };

                await _rootContext.TenantUserMappings.AddAsync(mapping);
                await ((DbContext)_rootContext).SaveChangesAsync();

                // Add some sample data for the tenant
                await SeedSampleDataAsync(tenantAdmin.Id);
            }
        }

        private async Task SeedSampleDataAsync(Guid createdBy)
        {
            // Add sample addresses if needed
            if (!await _context.Addresses.AnyAsync())
            {
                var sampleAddresses = new[]
                {
                    new Address
                    {
                        StreetAddress = "123 Main St",
                        City = "Sample City",
                        Country = "Sample Country",
                        PostalCode = "12345",
                        State = "Sample State",
                        CreatedBy = createdBy
                    },
                    new Address
                    {
                        StreetAddress = "456 Oak Ave",
                        City = "Another City",
                        Country = "Another Country",
                        PostalCode = "67890",
                        State = "Another State",
                        CreatedBy = createdBy
                    }
                };

                await _context.Addresses.AddRangeAsync(sampleAddresses);
                await _context.SaveChangesAsync();

                // Update some user profiles with addresses
                var profiles = await _context.UserProfiles.Take(2).ToListAsync();
                for (int i = 0; i < profiles.Count && i < sampleAddresses.Length; i++)
                {
                    profiles[i].AddressId = sampleAddresses[i].Id;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
} 