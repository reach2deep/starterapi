# Database Changes Guide

This document outlines the steps to add new tables and seed data in the multi-tenant architecture.

## 1. Adding New Tables

### 1.1 Core Project Changes
1. Create entity classes in appropriate namespaces:
   - Global entities: `starterkit.Core.Modules.Global`
   - Tenant entities: `starterkit.Core.Modules.Tenant`
   ```csharp
   // Example Global Entity
   public class ProductCategory : BaseEntity
   {
       public string Name { get; set; }
       public string Description { get; set; }
       public bool IsGlobal { get; set; }
   }

   // Example Tenant Entity
   public class Product : BaseEntity
   {
       public string Name { get; set; }
       public decimal Price { get; set; }
       public Guid CategoryId { get; set; }
       // Navigation property
       public virtual ProductCategory Category { get; set; }
   }
   ```

### 1.2 Infrastructure Project Changes
1. Create entity configurations:
   - Root DB: `Infrastructure/Persistence/RootDb/Configurations`
   - Tenant DB: `Infrastructure/Persistence/TenantDb/Configurations`
   ```csharp
   public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
   {
       public void Configure(EntityTypeBuilder<ProductCategory> builder)
       {
           builder.ToTable("ProductCategories");
           builder.HasKey(x => x.Id);
           builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
       }
   }
   ```

2. Add DbSet to appropriate DbContext:
   - Root DB: Add to `RootDbContext`
   - Tenant DB: Add to `TenantDbContext`
   ```csharp
   public DbSet<ProductCategory> ProductCategories { get; set; }
   ```

3. Register configurations in DbContext:
   ```csharp
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       base.OnModelCreating(modelBuilder);
       modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
   }
   ```

### 1.3 Application Project Changes
1. Update the corresponding DbContext interface:
   - For Root DB entities: Update `IRootDbContext`
   - For Tenant DB entities: Update `ITenantDbContext`
   ```csharp
   // In IRootDbContext.cs
   public interface IRootDbContext : IDisposable
   {
       // Existing DbSets...
       DbSet<NewEntity> NewEntities { get; set; }
   }

   // In ITenantDbContext.cs
   public interface ITenantDbContext : IDisposable
   {
       // Existing DbSets...
       DbSet<NewTenantEntity> NewTenantEntities { get; set; }
   }
   ```
   This step is crucial as it maintains the contract between the application and infrastructure layers and enables proper dependency injection.

### 1.4 Create and Apply Migrations
1. For Root Database:
   ```bash
   cd src/starterkit.Infrastructure
   dotnet ef migrations add AddProductCategories -c RootDbContext -o Persistence/RootDb/Migrations
   ```

2. For Tenant Database:
   ```bash
   cd src/starterkit.Infrastructure
   dotnet ef migrations add AddProducts -c TenantDbContext -o Persistence/TenantDb/Migrations
   ```

## 2. Adding Seed Data

### 2.1 Root Database Seeding
1. Modify `RootDbSeeder.cs`:
   - Add a new seeding method for your entity
   - Call it in the `SeedAsync()` method
   - Always check if data exists before seeding
   ```csharp
   public async Task SeedAsync()
   {
       await _context.Database.MigrateAsync();
       
       // Existing seeding
       await SeedGlobalUsersAsync();
       await SeedTenantsAsync();
       
       // New seeding method
       await SeedNewEntityAsync();
       
       await _context.SaveChangesAsync();
   }

   private async Task SeedNewEntityAsync()
   {
       if (!await _context.NewEntities.AnyAsync())
       {
           var entities = new[]
           {
               new NewEntity { /* properties */ }
           };
           await _context.NewEntities.AddRangeAsync(entities);
       }
   }
   ```

### 2.2 Tenant Database Seeding
1. Modify `TenantDbSeeder.cs`:
   - Add a new seeding method for your entity
   - Call it in the `SeedAsync()` method
   - For default/system data, seed before user-related data
   ```csharp
   public async Task SeedAsync()
   {
       try
       {
           await _context.Database.MigrateAsync();

           // Seed system/default data first
           await SeedDefaultDataAsync();

           // Then seed user-related data
           if (!await _context.Users.AnyAsync())
           {
               await SeedUsersAsync();
           }
       }
       catch (Exception ex)
       {
           throw new Exception($"Error seeding tenant database {_tenantId}: {ex.Message}", ex);
       }
   }

   private async Task SeedDefaultDataAsync()
   {
       if (!await _context.NewEntities.AnyAsync())
       {
           var defaultData = new[]
           {
               new NewEntity 
               { 
                   Name = "Default Item",
                   IsDefault = true,
                   CreatedBy = Guid.Empty // System
               }
           };
           await _context.NewEntities.AddRangeAsync(defaultData);
           await _context.SaveChangesAsync();
       }
   }
   ```

### 2.3 Seeding Best Practices
1. Order of Operations:
   - Always migrate database first
   - Seed system/default data before user-related data
   - Handle dependencies in correct order

2. Data Checks:
   - Always check if data exists before seeding
   - Use transactions for related data
   - Set proper CreatedBy values (Guid.Empty for system data)

3. Error Handling:
   - Wrap seeding in try-catch blocks
   - Log errors but allow other seeding to continue
   - Maintain data consistency

4. Default Data:
   - Use IsDefault or similar flag for system data
   - Document default values
   - Consider environment-specific defaults

## 3. Best Practices

### 3.1 Data Separation
- Keep global, shared data in Root database
- Keep tenant-specific data in Tenant databases
- Use consistent IDs across databases when needed
- Consider using enums for static reference data

### 3.2 Seeding Guidelines
- Always check if data exists before seeding
- Use transactions for related data
- Keep seeding methods focused and separate
- Handle errors gracefully
- Log seeding operations
- Use environment variables to control sample data seeding

### 3.3 Database Relationships
- Root DB can reference only Root DB tables
- Tenant DB can reference both Root DB and Tenant DB tables
- Use consistent IDs when referencing across databases
- Consider caching frequently accessed Root DB reference data

### 3.4 Migration Guidelines
- Keep migrations small and focused
- Test migrations with sample data
- Have rollback plan for each migration
- Document breaking changes
- Consider data backup before applying migrations

## 4. Troubleshooting

### 4.1 Common Issues
1. Migration fails
   - Ensure all required properties are set in entity configuration
   - Check for circular dependencies
   - Verify connection strings

2. Seeding fails
   - Check for null reference data
   - Verify foreign key constraints
   - Ensure proper order of seeding operations

3. Data inconsistency
   - Verify ID generation and sharing between databases
   - Check transaction boundaries
   - Validate data constraints

### 4.2 Debugging Tips
1. Enable detailed logging during migrations and seeding
2. Use transactions to maintain data consistency
3. Implement retry logic for transient failures
4. Add proper error handling and logging
5. Test with representative data volumes 