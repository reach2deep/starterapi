using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using starterkit.Core.Enums;
using starterkit.Core.Modules.Global;
using starterkit.Core.Modules.Tenant;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Infrastructure.Services;

namespace starterkit.Infrastructure.Data.TenantDb
{
    public class TenantDbSeeder : IDataSeeder
    {
        private readonly TenantDbContext _context;
        private readonly RootDbContext _rootContext;
        private readonly IPasswordHashService _passwordHashService;
        private readonly ILogger<TenantDbSeeder> _logger;
        private readonly string _tenantId;

        public TenantDbSeeder(
            TenantDbContext context,
            RootDbContext rootContext,
            IPasswordHashService passwordHashService,
            ILogger<TenantDbSeeder> logger,
            string tenantId)
        {
            _context = context;
            _rootContext = rootContext;
            _passwordHashService = passwordHashService;
            _logger = logger;
            _tenantId = tenantId;
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding for tenant {TenantId}", _tenantId);

                // Ensure database is migrated
                _logger.LogInformation("Applying migrations for tenant {TenantId}", _tenantId);
                await _context.Database.MigrateAsync();

                // Seed default permissions first
                _logger.LogInformation("Seeding default permissions for tenant {TenantId}", _tenantId);
                await SeedDefaultPermissionsAsync();
                
                // Seed default roles
                _logger.LogInformation("Seeding default roles for tenant {TenantId}", _tenantId);
                await SeedDefaultRolesAsync();

                // Assign default permissions to roles
                _logger.LogInformation("Assigning default permissions to roles for tenant {TenantId}", _tenantId);
                await AssignDefaultPermissionsToRolesAsync();

                if (!await _context.Users.AnyAsync())
                {
                    _logger.LogInformation("Seeding users for tenant {TenantId}", _tenantId);
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
                            FirstName = rootAdmin.FirstName,
                            LastName = rootAdmin.LastName,
                            CreatedBy = rootAdmin.Id
                        }
                    };

                    await _context.Users.AddAsync(rootAdminInTenant);
                    await _context.SaveChangesAsync();

                    // First create tenant admin in GlobalUsers
                    var tenantAdminGlobal = new GlobalUser
                    {
                        Email = $"admin@{tenant.Name.ToLower()}.com",
                        FirstName = $"{tenant.Name}",
                        LastName = "Admin",
                        PasswordHash = _passwordHashService.HashPassword("Admin@123"),
                        UserType = UserType.User,
                        Status = UserStatus.Active,
                        CreatedBy = rootAdmin.Id
                    };

                    await _rootContext.GlobalUsers.AddAsync(tenantAdminGlobal);
                    await _rootContext.SaveChangesAsync(); // Save to get the ID

                    // Create tenant admin in tenant database
                    var tenantAdmin = new User
                    {
                        Id = tenantAdminGlobal.Id, // Use the same ID as global user
                        Email = tenantAdminGlobal.Email,
                        FullName = $"{tenantAdminGlobal.FirstName} {tenantAdminGlobal.LastName}",
                        PasswordHash = tenantAdminGlobal.PasswordHash,
                        Status = UserStatus.Active,
                        CreatedBy = rootAdminInTenant.Id,
                        Profile = new UserProfile
                        {
                            FirstName = tenantAdminGlobal.FirstName,
                            LastName = tenantAdminGlobal.LastName,
                            CreatedBy = rootAdminInTenant.Id
                        }
                    };

                    await _context.Users.AddAsync(tenantAdmin);
                    await _context.SaveChangesAsync();

                    // Create tenant-user mapping in root database
                    var mapping = new TenantUserMapping
                    {
                        TenantId = tenant.Id,
                        UserId = tenantAdminGlobal.Id,
                        Role = "Admin",
                        CreatedBy = rootAdmin.Id
                    };

                    await _rootContext.TenantUserMappings.AddAsync(mapping);
                    await _rootContext.SaveChangesAsync();
                }

                _logger.LogInformation("Successfully completed seeding for tenant {TenantId}", _tenantId);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while seeding tenant {TenantId}. Error: {Message}, Inner Error: {InnerMessage}", 
                    _tenantId, ex.Message, ex.InnerException?.Message);
                throw new Exception($"Error seeding tenant database {_tenantId}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while seeding tenant {TenantId}. Error: {Message}", _tenantId, ex.Message);
                throw;
            }
        }

        private async Task SeedDefaultPermissionsAsync()
        {
            try
            {
                if (!await _context.Permissions.AnyAsync())
                {
                    _logger.LogInformation("Creating default permissions for tenant {TenantId}", _tenantId);
                    var defaultPermissions = new[]
                    {
                        // User Management
                        new Permission { Name = "View Users", Module = "Users", Action = "View", IsDefault = true, Description = "Allows viewing the list of users and user details" },
                        new Permission { Name = "Create User", Module = "Users", Action = "Create", IsDefault = true, Description = "Allows creating new users in the system" },
                        new Permission { Name = "Edit User", Module = "Users", Action = "Edit", IsDefault = true, Description = "Allows editing existing user information" },
                        new Permission { Name = "Delete User", Module = "Users", Action = "Delete", IsDefault = true, Description = "Allows deleting users from the system" },

                        // Role Management
                        new Permission { Name = "View Roles", Module = "Roles", Action = "View", IsDefault = true, Description = "Allows viewing the list of roles and role details" },
                        new Permission { Name = "Create Role", Module = "Roles", Action = "Create", IsDefault = true, Description = "Allows creating new roles in the system" },
                        new Permission { Name = "Edit Role", Module = "Roles", Action = "Edit", IsDefault = true, Description = "Allows editing existing role information" },
                        new Permission { Name = "Delete Role", Module = "Roles", Action = "Delete", IsDefault = true, Description = "Allows deleting roles from the system" },

                        // Permission Management
                        new Permission { Name = "View Permissions", Module = "Permissions", Action = "View", IsDefault = true, Description = "Allows viewing the list of permissions and their details" },
                        new Permission { Name = "Assign Permissions", Module = "Permissions", Action = "Assign", IsDefault = true, Description = "Allows assigning permissions to roles" }
                    };

                    foreach (var permission in defaultPermissions)
                    {
                        permission.CreatedBy = Guid.Empty;
                        _logger.LogDebug("Adding permission: {PermissionName} ({Module}.{Action})", 
                            permission.Name, permission.Module, permission.Action);
                    }

                    await _context.Permissions.AddRangeAsync(defaultPermissions);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully added {Count} default permissions", defaultPermissions.Length);
                }
                else
                {
                    _logger.LogInformation("Permissions already exist for tenant {TenantId}, skipping", _tenantId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding permissions for tenant {TenantId}", _tenantId);
                throw;
            }
        }

        private async Task AssignDefaultPermissionsToRolesAsync()
        {
            try
            {
                var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Tenant Admin");
                var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

                if (adminRole == null)
                {
                    _logger.LogWarning("Admin role not found for tenant {TenantId}", _tenantId);
                    return;
                }

                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id))
                {
                    _logger.LogInformation("Assigning all permissions to admin role for tenant {TenantId}", _tenantId);
                    var allPermissions = await _context.Permissions.ToListAsync();
                    var adminPermissions = allPermissions.Select(p => new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = p.Id,
                        CreatedBy = Guid.Empty
                    });

                    await _context.RolePermissions.AddRangeAsync(adminPermissions);
                    _logger.LogInformation("Added {Count} permissions to admin role", allPermissions.Count);
                }

                if (userRole != null && !await _context.RolePermissions.AnyAsync(rp => rp.RoleId == userRole.Id))
                {
                    _logger.LogInformation("Assigning view permissions to user role for tenant {TenantId}", _tenantId);
                    var viewPermissions = await _context.Permissions
                        .Where(p => p.Action == "View")
                        .ToListAsync();

                    var userPermissions = viewPermissions.Select(p => new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = p.Id,
                        CreatedBy = Guid.Empty
                    });

                    await _context.RolePermissions.AddRangeAsync(userPermissions);
                    _logger.LogInformation("Added {Count} view permissions to user role", viewPermissions.Count);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning permissions to roles for tenant {TenantId}", _tenantId);
                throw;
            }
        }

        private async Task SeedDefaultRolesAsync()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var defaultRoles = new[]
                {
                    new Role
                    {
                        Name = "Tenant Admin",
                        Description = "Tenant administrator role with full access",
                        IsDefault = true,
                        CreatedBy = Guid.Empty
                    },
                    new Role
                    {
                        Name = "User",
                        Description = "Regular user role with standard access",
                        IsDefault = true,
                        CreatedBy = Guid.Empty
                    }
                };

                await _context.Roles.AddRangeAsync(defaultRoles);
                await _context.SaveChangesAsync();
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
                        FirstName = globalUser.FirstName,
                        LastName = globalUser.LastName,
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