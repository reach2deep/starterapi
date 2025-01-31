using Microsoft.EntityFrameworkCore;
using starterkit.Core.Enums;
using starterkit.Core.Modules.Global;
using starterkit.Infrastructure.Services;


namespace starterkit.Infrastructure.Data.RootDb
{
    public class RootDbSeeder : IDataSeeder
    {
        private readonly RootDbContext _context;
        private readonly IPasswordHashService _passwordHashService;

        public RootDbSeeder(
            RootDbContext context,
            IPasswordHashService passwordHashService)
        {
            _context = context;
            _passwordHashService = passwordHashService;
        }

        public async Task SeedAsync()
        {
            // Ensure database is recreated with latest schema
            //await _context.Database.EnsureDeletedAsync();
            await _context.Database.MigrateAsync();

            GlobalUser rootAdmin = null;
            if (!await _context.GlobalUsers.AnyAsync())
            {
                // Add root admin
                rootAdmin = new GlobalUser
                {
                    Email = "rootadmin@example.com",
                    FirstName = "Root",
                    LastName = "Admin",
                    PasswordHash = _passwordHashService.HashPassword("Admin@123"),
                    UserType = UserType.RootAdmin,
                    Status = UserStatus.Active,
                    CreatedBy = Guid.Empty // System
                };

                await _context.GlobalUsers.AddAsync(rootAdmin);
                await _context.SaveChangesAsync(); // Save to generate Id
            }
            else
            {
                rootAdmin = await _context.GlobalUsers.FirstOrDefaultAsync(u => u.UserType == UserType.RootAdmin);
            }

            if (!await _context.Tenants.AnyAsync())
            {
                // Add default tenants
                var tenants = new[]
                {
                    new Tenant
                    {
                        Name = "Alpha",
                        DatabaseName = "alpha_tenant",
                        ConnectionString = "Server=localhost;Database=alpha_tenant;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true;",
                        Status = TenantStatus.Active,
                        CreatedBy = Guid.Empty, // System
                        Description = "Alpha tenant description"
                    },
                    new Tenant
                    {
                        Name = "Beta",
                        DatabaseName = "beta_tenant",
                        ConnectionString = "Server=localhost;Database=beta_tenant;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;MultipleActiveResultSets=true;",
                        Status = TenantStatus.Active,
                        CreatedBy = Guid.Empty, // System
                        Description = "Beta tenant description"
                    }
                };

                await _context.Tenants.AddRangeAsync(tenants);
                await _context.SaveChangesAsync(); // Save to generate Ids

                // Create tenant-user mappings for root admin
                if (rootAdmin != null)
                {
                    foreach (var tenant in tenants)
                    {
                        var mapping = new TenantUserMapping
                        {
                            TenantId = tenant.Id,
                            UserId = rootAdmin.Id,
                            Role = "Admin", // Root admin gets admin role in all tenants
                            CreatedBy = Guid.Empty // System
                        };
                        await _context.Set<TenantUserMapping>().AddAsync(mapping);
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}