using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using starterkit.Core.Enums;
using starterkit.Core.Modules.Global;
using starterkit.Core.Modules.Tenant;
using starterkit.Infrastructure.Data.RootDb;
using starterkit.Infrastructure.Persistence.RootDb;
using starterkit.Infrastructure.Persistence.TenantDb;
using starterkit.Infrastructure.Services;
using starterkit.Core.Interfaces.Services;

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

                // Migrations are already handled by TenantDatabaseInitializer
                // No need to migrate again here

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
                    
                    // Get the Tenant Admin role
                    var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Tenant Admin");
                    if (adminRole != null)
                    {
                        // Create user role mapping
                        var userRole = new UserRole
                        {
                            UserId = tenantAdmin.Id,
                            RoleId = adminRole.Id,
                            CreatedBy = rootAdminInTenant.Id
                        };
                        await _context.UserRoles.AddAsync(userRole);
                    }
                    else
                    {
                        _logger.LogWarning("Tenant Admin role not found when creating tenant admin user for tenant {TenantId}", _tenantId);
                    }

                    await _context.SaveChangesAsync();

                    // Create tenant-user mapping in root database with Admin role
                    var mapping = new TenantUserMapping
                    {
                        TenantId = tenant.Id,
                        UserId = tenantAdminGlobal.Id,
                        Role = "Tenant Admin", // Updated to match the role name
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
                _logger.LogInformation("Creating/Updating default permissions for tenant {TenantId}", _tenantId);
                var defaultPermissions = new[]
                {
                    new Permission { Name = "Tenant Management", Module = "TenantManagement", Action = "Module", IsDefault = true, Description = "User Management Module", Parent = null },
                    
                    // User Management Module
                    new Permission { Name = "User Management", Module = "UserManagement", Action = "Module", IsDefault = true, Description = "User Management Module", Parent = "TenantManagement" },
                    new Permission { Name = "View Users", Module = "Users", Action = "View", IsDefault = true, Description = "Allows viewing the list of users and user details", Parent = "UserManagement" },
                    new Permission { Name = "Create User", Module = "Users", Action = "Create", IsDefault = true, Description = "Allows creating new users in the system", Parent = "UserManagement" },
                    new Permission { Name = "Edit User", Module = "Users", Action = "Edit", IsDefault = true, Description = "Allows editing existing user information", Parent = "UserManagement" },
                    new Permission { Name = "Delete User", Module = "Users", Action = "Delete", IsDefault = true, Description = "Allows deleting users from the system", Parent = "UserManagement" },

                    // Role Management Module
                    new Permission { Name = "Role Management", Module = "RoleManagement", Action = "Module", IsDefault = true, Description = "Role Management Module", Parent = "TenantManagement" },
                    new Permission { Name = "View Roles", Module = "Roles", Action = "View", IsDefault = true, Description = "Allows viewing the list of roles and role details", Parent = "RoleManagement" },
                    new Permission { Name = "Create Role", Module = "Roles", Action = "Create", IsDefault = true, Description = "Allows creating new roles in the system", Parent = "RoleManagement" },
                    new Permission { Name = "Edit Role", Module = "Roles", Action = "Edit", IsDefault = true, Description = "Allows editing existing role information", Parent = "RoleManagement" },
                    new Permission { Name = "Delete Role", Module = "Roles", Action = "Delete", IsDefault = true, Description = "Allows deleting roles from the system", Parent = "RoleManagement" },

                    // Permission Management Module
                    new Permission { Name = "Permission Management", Module = "PermissionManagement", Action = "Module", IsDefault = true, Description = "Permission Management Module", Parent = "TenantManagement" },
                    new Permission { Name = "View Permissions", Module = "Permissions", Action = "View", IsDefault = true, Description = "Allows viewing the list of permissions and their details", Parent = "PermissionManagement" },
                    new Permission { Name = "Assign Permissions", Module = "Permissions", Action = "Assign", IsDefault = true, Description = "Allows assigning permissions to roles", Parent = "PermissionManagement" },

                    // Society Management Module
                    new Permission { Name = "Society Management", Module = "SocietyManagement", Action = "Module", IsDefault = true, Description = "Society Management Module", Parent = "TenantManagement" },
                    
                    // Society Permissions
                    new Permission { Name = "View Societies", Module = "Societies", Action = "View", IsDefault = true, Description = "Allows viewing the list of societies and their details", Parent = "SocietyManagement" },
                    new Permission { Name = "Create Society", Module = "Societies", Action = "Create", IsDefault = true, Description = "Allows creating new societies", Parent = "SocietyManagement" },
                    new Permission { Name = "Edit Society", Module = "Societies", Action = "Edit", IsDefault = true, Description = "Allows editing existing society information", Parent = "SocietyManagement" },
                    new Permission { Name = "Delete Society", Module = "Societies", Action = "Delete", IsDefault = true, Description = "Allows deleting societies", Parent = "SocietyManagement" },

                    // Block Permissions
                    new Permission { Name = "View Blocks", Module = "Blocks", Action = "View", IsDefault = true, Description = "Allows viewing the list of blocks and their details", Parent = "SocietyManagement" },
                    new Permission { Name = "Create Block", Module = "Blocks", Action = "Create", IsDefault = true, Description = "Allows creating new blocks in societies", Parent = "SocietyManagement" },
                    new Permission { Name = "Edit Block", Module = "Blocks", Action = "Edit", IsDefault = true, Description = "Allows editing existing block information", Parent = "SocietyManagement" },
                    new Permission { Name = "Delete Block", Module = "Blocks", Action = "Delete", IsDefault = true, Description = "Allows deleting blocks", Parent = "SocietyManagement" },

                    // Floor Permissions
                    new Permission { Name = "View Floors", Module = "Floors", Action = "View", IsDefault = true, Description = "Allows viewing the list of floors and their details", Parent = "SocietyManagement" },
                    new Permission { Name = "Create Floor", Module = "Floors", Action = "Create", IsDefault = true, Description = "Allows creating new floors in blocks", Parent = "SocietyManagement" },
                    new Permission { Name = "Edit Floor", Module = "Floors", Action = "Edit", IsDefault = true, Description = "Allows editing existing floor information", Parent = "SocietyManagement" },
                    new Permission { Name = "Delete Floor", Module = "Floors", Action = "Delete", IsDefault = true, Description = "Allows deleting floors", Parent = "SocietyManagement" },

                    // Unit Permissions
                    new Permission { Name = "View Units", Module = "Units", Action = "View", IsDefault = true, Description = "Allows viewing the list of units and their details", Parent = "SocietyManagement" },
                    new Permission { Name = "Create Unit", Module = "Units", Action = "Create", IsDefault = true, Description = "Allows creating new units in floors", Parent = "SocietyManagement" },
                    new Permission { Name = "Edit Unit", Module = "Units", Action = "Edit", IsDefault = true, Description = "Allows editing existing unit information", Parent = "SocietyManagement" },
                    new Permission { Name = "Delete Unit", Module = "Units", Action = "Delete", IsDefault = true, Description = "Allows deleting units", Parent = "SocietyManagement" },

                    // Unit Ownership Permissions
                    new Permission { Name = "View Unit Ownerships", Module = "UnitOwnerships", Action = "View", IsDefault = true, Description = "Allows viewing unit ownership records", Parent = "SocietyManagement" },
                    new Permission { Name = "Create Unit Ownership", Module = "UnitOwnerships", Action = "Create", IsDefault = true, Description = "Allows creating new unit ownership records", Parent = "SocietyManagement" },
                    new Permission { Name = "Edit Unit Ownership", Module = "UnitOwnerships", Action = "Edit", IsDefault = true, Description = "Allows editing unit ownership records", Parent = "SocietyManagement" },
                    new Permission { Name = "Delete Unit Ownership", Module = "UnitOwnerships", Action = "Delete", IsDefault = true, Description = "Allows deleting unit ownership records", Parent = "SocietyManagement" },
                    new Permission { Name = "Transfer Unit Ownership", Module = "UnitOwnerships", Action = "Transfer", IsDefault = true, Description = "Allows transferring unit ownership", Parent = "SocietyManagement" },

                    // Unit Resident Permissions
                    new Permission { Name = "View Unit Residents", Module = "UnitResidents", Action = "View", IsDefault = true, Description = "Allows viewing unit resident records", Parent = "SocietyManagement" },
                    new Permission { Name = "Create Unit Resident", Module = "UnitResidents", Action = "Create", IsDefault = true, Description = "Allows creating new unit resident records", Parent = "SocietyManagement" },
                    new Permission { Name = "Edit Unit Resident", Module = "UnitResidents", Action = "Edit", IsDefault = true, Description = "Allows editing unit resident records", Parent = "SocietyManagement" },
                    new Permission { Name = "Delete Unit Resident", Module = "UnitResidents", Action = "Delete", IsDefault = true, Description = "Allows deleting unit resident records", Parent = "SocietyManagement" },
                    new Permission { Name = "Manage Primary Resident", Module = "UnitResidents", Action = "ManagePrimary", IsDefault = true, Description = "Allows managing primary resident status", Parent = "SocietyManagement" },
                    new Permission { Name = "Move Out Resident", Module = "UnitResidents", Action = "MoveOut", IsDefault = true, Description = "Allows moving out residents from units", Parent = "SocietyManagement" }
                };

                // Get existing permissions
                var existingPermissions = await _context.Permissions.ToListAsync();
                var existingModules = existingPermissions.Select(p => p.Module).Distinct().ToList();

                // Find new permissions to add
                var newPermissions = defaultPermissions
                    .Where(dp => !existingPermissions.Any(ep => 
                        ep.Module == dp.Module && 
                        ep.Action == dp.Action &&
                        ep.Name == dp.Name))
                    .ToList();

                if (newPermissions.Any())
                {
                    _logger.LogInformation("Found {Count} new permissions to add for tenant {TenantId}", 
                        newPermissions.Count, _tenantId);

                    foreach (var permission in newPermissions)
                    {
                        permission.CreatedBy = Guid.Empty;
                        _logger.LogDebug("Adding new permission: {PermissionName} ({Module}.{Action})", 
                            permission.Name, permission.Module, permission.Action);
                    }

                    await _context.Permissions.AddRangeAsync(newPermissions);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully added {Count} new permissions", newPermissions.Count);

                    // Re-assign permissions to admin role
                    await AssignDefaultPermissionsToRolesAsync();
                }
                else
                {
                    _logger.LogInformation("No new permissions to add for tenant {TenantId}", _tenantId);
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