using Microsoft.EntityFrameworkCore;
using starterkit.Core.Entities.Global;
using starterkit.Core.Entities.Tenant;
using starterkit.Core.Enums;
using starterkit.Core.Interfaces.Data;
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
            try
            {
                // Ensure database is migrated
                await _context.Database.MigrateAsync();

                if (!await _context.Users.AnyAsync())
                {
                    // Get tenant info from root database
                    var tenant = await _rootContext.Tenants
                        .FirstOrDefaultAsync(t => t.DatabaseName == _tenantId);
                    if (tenant == null)
                    {
                        throw new Exception($"Tenant {_tenantId} not found in root database");
                    }

                    // Get root admin from root database
                    var rootAdmin = await _rootContext.GlobalUsers
                        .FirstOrDefaultAsync(u => u.UserType == UserType.RootAdmin);
                    if (rootAdmin == null)
                    {
                        throw new Exception("Root admin not found in root database");
                    }

                    // Create root admin in tenant database with same ID
                    var rootAdminInTenant = new User
                    {
                        Id = rootAdmin.Id,
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
                    await _context.SaveChangesAsync();

                    // Create tenant admin
                    var tenantAdmin = new User
                    {
                        Email = $"admin@{tenant.Name.ToLower()}.com",
                        FullName = $"{tenant.Name} Admin",
                        PasswordHash = _passwordHashService.HashPassword("Admin@123"),
                        Status = UserStatus.Active,
                        CreatedBy = rootAdminInTenant.Id,
                        Profile = new UserProfile
                        {
                            CreatedBy = rootAdminInTenant.Id
                        }
                    };

                    await _context.Users.AddAsync(tenantAdmin);
                    await _context.SaveChangesAsync();

                    // Create tenant-user mapping in root database
                    var mapping = new TenantUserMapping
                    {
                        TenantId = tenant.Id,
                        UserId = tenantAdmin.Id,
                        Role = "Admin",
                        CreatedBy = rootAdmin.Id
                    };

                    await _rootContext.TenantUserMappings.AddAsync(mapping);
                    await _rootContext.SaveChangesAsync();

                    // Add some sample data
                    //await SeedSampleDataAsync(tenantAdmin.Id, tenant.Id, rootAdmin.Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding tenant database {_tenantId}: {ex.Message}", ex);
            }
        }

        private async Task SeedSampleDataAsync(Guid createdBy, Guid tenantId, Guid rootAdminId)
        {
            // First create global users
            var globalUsers = new List<GlobalUser>
            {
                new GlobalUser
                {
                    Email = $"user1@{_tenantId}.com",
                    FirstName = "Sample",
                    LastName = "User 1",
                    PasswordHash = _passwordHashService.HashPassword("User@123"),
                    UserType = UserType.User,
                    Status = UserStatus.Active,
                    CreatedBy = rootAdminId
                },
                new GlobalUser
                {
                    Email = $"user2@{_tenantId}.com",
                    FirstName = "Sample",
                    LastName = "User 2",
                    PasswordHash = _passwordHashService.HashPassword("User@123"),
                    UserType = UserType.User,
                    Status = UserStatus.Active,
                    CreatedBy = rootAdminId
                }
            };

            await _rootContext.GlobalUsers.AddRangeAsync(globalUsers);
            await _rootContext.SaveChangesAsync();

            // Then create tenant users with the same IDs
            var users = new List<User>();
            foreach (var globalUser in globalUsers)
            {
                var user = new User
                {
                    Id = globalUser.Id, // Use same ID as global user
                    Email = globalUser.Email,
                    FullName = $"{globalUser.FirstName} {globalUser.LastName}",
                    PasswordHash = globalUser.PasswordHash,
                    Status = UserStatus.Active,
                    CreatedBy = createdBy,
                    Profile = new UserProfile
                    {
                        DateOfBirth = DateTime.UtcNow.AddYears(-25 - users.Count * 5), // Different ages
                        CreatedBy = createdBy,
                        Address = new Address
                        {
                            StreetAddress = $"{123 + users.Count * 333} Main St",
                            City = $"Sample City {users.Count + 1}",
                            State = $"State {users.Count + 1}",
                            Country = $"Country {users.Count + 1}",
                            PostalCode = $"{12345 + users.Count * 55555}",
                            CreatedBy = createdBy
                        }
                    }
                };
                users.Add(user);
            }

            await _context.Users.AddRangeAsync(users);
            await _context.SaveChangesAsync();

            // Create tenant-user mappings
            var mappings = users.Select(u => new TenantUserMapping
            {
                TenantId = tenantId,
                UserId = u.Id,
                Role = "User",
                CreatedBy = createdBy
            }).ToList();

            await _rootContext.TenantUserMappings.AddRangeAsync(mappings);
            await _rootContext.SaveChangesAsync();
        }
    }
} 