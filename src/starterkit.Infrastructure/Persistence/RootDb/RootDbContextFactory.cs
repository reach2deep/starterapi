using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace starterkit.Infrastructure.Data.RootDb
{
    public class RootDbContextFactory : IDesignTimeDbContextFactory<RootDbContext>
    {
        public RootDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<RootDbContext>();
            var connectionString = configuration.GetConnectionString("RootConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new RootDbContext(optionsBuilder.Options);
        }
    }
} 