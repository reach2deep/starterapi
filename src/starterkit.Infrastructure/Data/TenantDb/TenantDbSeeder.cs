using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;
using starterkit.Core.Entities.Tenant;
using starterkit.Core.Enums;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Services;

namespace starterkit.Infrastructure.Data.TenantDb
{
    public class TenantDbSeeder : IDataSeeder
    {
        private readonly TenantDbContext _context;
        private readonly RootDbContext _rootContext;
        private readonly IPasswordHashService _passwordHashService;
        private readonly string _tenantId;

        public TenantDbSeeder(
            TenantDbContext context, 
            RootDbContext rootContext,
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
            // Ensure database is recreated with latest schema
            //await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();

            if (!await _context.Users.AnyAsync())
            {
                // Get tenant info from root database
                var tenant = await _rootContext.Tenants.FirstOrDefaultAsync(t => t.DatabaseName == _tenantId);
                if (tenant == null)
                {
                    throw new Exception($"Tenant {_tenantId} not found in root database");
                }

                // Create tenant admin in tenant database
                var tenantAdmin = new User
                {
                    Email = $"admin@{tenant.Name.ToLower()}.com",
                    FullName = $"{tenant.Name} Admin",
                    PasswordHash = _passwordHashService.HashPassword("Admin@123"), // Use proper password hashing
                    Status = UserStatus.Active,
                    CreatedBy = Guid.Empty, // System
                    Profile = new UserProfile
                    {
                        CreatedBy = Guid.Empty // System
                    }
                };

                await _context.Users.AddAsync(tenantAdmin);
                await _context.SaveChangesAsync();

                // Create global user for tenant admin
                var globalUser = new GlobalUser
                {
                    Email = tenantAdmin.Email,
                    FirstName = tenant.Name,
                    LastName = "Admin",
                    PasswordHash = tenantAdmin.PasswordHash,
                    UserType = UserType.TenantAdmin,
                    Status = UserStatus.Active,
                    CreatedBy = Guid.Empty // System
                };

                await _rootContext.GlobalUsers.AddAsync(globalUser);
                await _rootContext.SaveChangesAsync();

                // Create tenant-user mapping in root database
                var mapping = new TenantUserMapping
                {
                    TenantId = tenant.Id,
                    UserId = globalUser.Id,
                    Role = "Admin",
                    CreatedBy = Guid.Empty // System
                };

                await _rootContext.Set<TenantUserMapping>().AddAsync(mapping);
                await _rootContext.SaveChangesAsync();

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