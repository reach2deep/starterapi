# Feature Implementation Guide

## Pre-Implementation Analysis
1. **Understand Requirements**
   - Review the feature requirements thoroughly
   - Identify the entities and relationships needed
   - Map out the API endpoints required
   - Consider multi-tenancy implications

2. **Review Existing Code**
   - Check similar implementations in the project
   - Identify reusable components
   - Note the patterns used in similar features

## Implementation Steps

### 1. Domain Layer (Core)
```csharp
// Example: Society.cs in Core layer
public class Society : BaseEntity
{
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    // ... other properties
    public virtual ICollection<Block> Blocks { get; set; }
}
```
- Define entities with proper properties and relationships
- Add data annotations for basic validation
- Include XML documentation
- Define repository interfaces

### 2. Data Transfer Objects (DTOs)
```csharp
// Example: Request DTOs
public class CreateSocietyRequest
{
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public AddressDto Address { get; set; }
    // ... other properties
}

// Example: Response DTOs
public class SocietyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public AddressDto Address { get; set; }
    public DateTime CreatedAt { get; set; }
    // ... other properties
}
```
- Create separate DTOs for requests and responses
- Keep DTOs focused on API contract needs and always ends with Request or Response
- Include documentation for all DTO properties
- Use AutoMapper for entity-DTO mappings

### 3. Infrastructure Layer
```csharp
// Example: Entity Configuration
public class SocietyConfiguration : IEntityTypeConfiguration<Society>
{
    public void Configure(EntityTypeBuilder<Society> builder)
    {
        builder.ToTable("Societies");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        // ... other configurations
    }
}

// Example: Repository Implementation
public class SocietyRepository : ISocietyRepository
{
    private readonly TenantDbContext _context;
    public SocietyRepository(TenantDbContext context) => _context = context;
    // ... implement interface methods
}
```
- Create entity configurations
- Implement repositories
- Add migrations
- Register dependencies

### 4. Application Layer
```csharp
// Example: Service Interface
public interface ISocietyService
{
    Task<ApiResponse<Society>> GetByIdAsync(Guid id);
    // ... other service methods
}

// Example: Service Implementation
public class SocietyService : ISocietyService
{
    private readonly ISocietyRepository _repository;
    private readonly ILogger<SocietyService> _logger;

    public SocietyService(ISocietyRepository repository, ILogger<SocietyService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    // ... implement interface methods with proper error handling
}
```
- Define service interfaces
- Implement services with business logic
- Add proper error handling and logging
- Use ApiResponse<T> for consistent responses

### 5. API Layer
```csharp
// Example: Controller
[Route("api/v1/tenant/[controller]")]
public class SocietyController : BaseApiController
{
    private readonly ISocietyService _service;

    public SocietyController(ISocietyService service) => _service = service;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _service.GetByIdAsync(id);
        return Ok(result);
    }
    // ... other endpoints
}
```
- Create controllers with proper routing
- Add API documentation
- Implement authorization if needed
- Use consistent response patterns

### 6. Dependency Injection
```csharp
// Example: Module Service Extensions
public static class SocietyManagementServiceExtensions
{
    public static IServiceCollection AddSocietyManagementModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ISocietyRepository, SocietyRepository>();
        
        // Register services
        services.AddScoped<ISocietyService, SocietyService>();
        
        // Register AutoMapper profiles
        services.AddAutoMapper(typeof(SocietyMappingProfile));
        
        return services;
    }
}
```
- Create extension method for module registration
- Register all dependencies in one place
- Follow consistent registration pattern:
  - Repositories first
  - Services next
  - Other dependencies last
- Use appropriate lifetimes (Scoped/Singleton/Transient)
- Keep registrations organized by module

### 7. Database Migration
```bash
# Create migration
dotnet ef migrations add AddSocietyManagementTables --context TenantDbContext --project starterkit.Infrastructure.csproj

# Verify migration file
# Check Up() and Down() methods
# Review entity configurations and relationships
```
- Create migration after all entities and configurations are in place
- Review the generated migration file to ensure:
  - All tables are created with proper columns
  - Relationships are properly configured
  - Indexes are created as specified
  - Constraints are properly set


## Best Practices
1. **Code Organization**
   - Follow existing module structure
   - Keep files focused and small
   - Use consistent naming

2. **Error Handling**
   - Use try-catch in services
   - Log errors appropriately
   - Return proper error responses

3. **Documentation**
   - Add XML comments
   - Document API endpoints
   - Update README if needed

4. **Multi-tenancy**
   - Use proper DbContext
   - Consider tenant isolation
   - Test with multiple tenants

## Verification Checklist
- [ ] Entity properly defined with relationships
- [ ] Repository interface and implementation complete
- [ ] Service interface and implementation with error handling
- [ ] Controller endpoints with proper documentation
- [ ] Entity configuration with proper constraints
- [ ] Migration created and verified
- [ ] Dependencies registered
- [ ] Multi-tenant considerations addressed

## Common Pitfalls to Avoid
1. Don't skip entity configuration
2. Don't forget error handling
3. Don't ignore multi-tenant implications
4. Don't skip documentation
5. Don't forget to test with different tenants
6. Don't ignore existing patterns
7. Don't skip validation
8. Don't forget proper logging 