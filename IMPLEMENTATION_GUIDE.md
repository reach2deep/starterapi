# Feature Implementation Guide

## Pre-Implementation Analysis
1. **Understand Requirements**
   - Review the feature requirements thoroughly
   - Identify the entities and relationships needed
   - Map out the API endpoints required get confirmation from the User
   - Consider multi-tenancy implications

2. **Review Existing Code**
   - Check similar implementations in the project
   - Identify reusable components
   - Note the patterns used in similar features

## Implementation Steps

### 1. Domain Models (Core Layer)
```csharp
// Example: Society.cs in Core layer
public class Society : BaseEntity
{
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public string ContactEmail { get; set; }
    public string ContactPhone { get; set; }
    public int TotalBlocks { get; set; }
    public Guid AddressId { get; set; }
    
    // Navigation properties
    public virtual Address Address { get; set; }
    public virtual ICollection<Block> Blocks { get; set; }
}
```
- Define entities with proper properties
- Set up relationships between entities
- Include XML documentation
- Keep models focused and clean

### 2. Repository Interfaces (Core Layer)
```csharp
public interface ISocietyRepository
{
    Task<IEnumerable<Society>> GetAllAsync();
    Task<Society> GetByIdAsync(Guid id);
    Task<Society> AddAsync(Society society);
    Task<Society> UpdateAsync(Society society);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
}
```
- Define clear repository interfaces
- Keep methods focused and atomic
- Use async/await pattern
- Include documentation

### 3. DbContext and Configurations (Infrastructure Layer)
```csharp
// DbContext Interface
public interface ITenantDbContext
{
    DbSet<Society> Societies { get; set; }
    DbSet<Block> Blocks { get; set; }
   
}

// DbContext Implementation
public class TenantDbContext : DbContext, ITenantDbContext
{
    public DbSet<Society> Societies { get; set; }
    public DbSet<Block> Blocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SocietyConfiguration());
        modelBuilder.ApplyConfiguration(new BlockConfiguration());
    }
}

// Entity Configuration
public class SocietyConfiguration : IEntityTypeConfiguration<Society>
{
    public void Configure(EntityTypeBuilder<Society> builder)
    {
        builder.ToTable("Societies");
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RegistrationNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.RegistrationNumber)
            .IsUnique();

        builder.HasMany(x => x.Blocks)
            .WithOne(x => x.Society)
            .HasForeignKey(x => x.SocietyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### 4. Database Migrations
```bash
# Create migration

# Root migrations (Only when there is a change in the RootDbContext)
dotnet ef migrations add InitialSchema -c RootDbContext -p src/starterkit.Infrastructure -s src/starterkit.API -o Persistence/RootDb/Migrations

# Tenant migrations
dotnet ef migrations add InitialSchema -c TenantDbContext -p src/starterkit.Infrastructure -s src/starterkit.API -o Persistence/TenantDb/Migrations

# Verify migration file
# Check Up() and Down() methods
# Review entity configurations and relationships
```
- Create initial migration
- Review generated SQL
- Verify relationships and constraints
- Add seed data if needed

### 5. Repository Implementation (Infrastructure Layer)
```csharp
public class SocietyRepository : ISocietyRepository
{
    private readonly ITenantDbContext _context;
    private readonly ILogger<SocietyRepository> _logger;

    public SocietyRepository(ITenantDbContext context, ILogger<SocietyRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
  
}

**Key Points for Repository Implementation:**
- Implement all interface methods
- Use proper exception handling and logging
- Include relevant entity includes (eager loading)
- Handle soft delete with IsActive flag
- Use async/await consistently
- Keep methods focused and clean
- Add XML documentation
- Follow repository pattern best practices

### 6. DTOs (Application Layer)
```csharp
// Request DTOs
public class CreateSocietyRequest
{
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public string ContactEmail { get; set; }
    public AddressDto Address { get; set; }
}

public class UpdateSocietyRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public string ContactEmail { get; set; }
    public AddressDto Address { get; set; }
}

// Response DTOs
public class SocietyResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RegistrationNumber { get; set; }
    public string ContactEmail { get; set; }
    public AddressDto Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
}
```

### 7. Validators (Application Layer)
```csharp
public class CreateSocietyRequestValidator : AbstractValidator<CreateSocietyRequest>
{
    public CreateSocietyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Address)
            .NotNull()
            .SetValidator(new AddressValidator());
    }
}
```

### 8. Service Layer (Application Layer)
```csharp
// Service Interface
public interface ISocietyService
{
    Task<ApiResponse<SocietyResponse>> GetByIdAsync(Guid id);
    Task<ApiResponse<SocietyResponse>> CreateAsync(CreateSocietyRequest request);
    Task<ApiResponse<SocietyResponse>> UpdateAsync(UpdateSocietyRequest request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}

// Service Implementation
public class SocietyService : ISocietyService
{
    private readonly ISocietyRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateSocietyRequest> _createValidator;

    public async Task<ApiResponse<SocietyResponse>> CreateAsync(CreateSocietyRequest request)
    {
        try
        {
            // 1. Validate request
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return ApiResponse<SocietyResponse>.CreateError(validationResult.Errors.First().ErrorMessage);

            // 2. Business rules
            if (await _repository.ExistsByNameAsync(request.Name))
                return ApiResponse<SocietyResponse>.CreateError("Society with this name already exists");

            // 3. Map and save
            var entity = _mapper.Map<Society>(request);
            var result = await _repository.AddAsync(entity);
            
            // 4. Return response
            var response = _mapper.Map<SocietyResponse>(result);
            return ApiResponse<SocietyResponse>.CreateSuccess(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating society");
            return ApiResponse<SocietyResponse>.CreateError("Failed to create society");
        }
    }
}
```

### 9. Controllers (API Layer)
```csharp
[Route("api/v1/tenant/societies")]
[Authorize]
public class SocietyController : BaseApiController
{
    private readonly ISocietyService _service;

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
    public async Task<IActionResult> Create([FromBody] CreateSocietyRequest request)
    {
        var result = await _service.CreateAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<SocietyResponse>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSocietyRequest request)
    {
        if (id != request.Id)
            return BadRequest("ID mismatch");

        var result = await _service.UpdateAsync(request);
        return Ok(result);
    }
}
```

### 10. Dependency Injection
```csharp
public static class SocietyManagementModule
{
    public static IServiceCollection AddSocietyManagementModule(this IServiceCollection services)
    {
        // Register repositories
        services.AddScoped<ISocietyRepository, SocietyRepository>();
        
        // Register services
        services.AddScoped<ISocietyService, SocietyService>();
        
        // Register validators
        services.AddScoped<IValidator<CreateSocietyRequest>, CreateSocietyRequestValidator>();
        services.AddScoped<IValidator<UpdateSocietyRequest>, UpdateSocietyRequestValidator>();
        
        // Register AutoMapper profiles
        services.AddAutoMapper(typeof(SocietyMappingProfile));
        
        return services;
    }
}
```

## Best Practices and Verification
1. **Code Organization**
   - Follow the layer order: Core → Infrastructure → Application → API
   - Keep files small and focused
   - Use consistent naming conventions

2. **Testing**
   - Write unit tests for validators and services
   - Write integration tests for repositories
   - Test multi-tenant scenarios

3. **Documentation**
   - Add XML comments to all public members
   - Document API endpoints with Swagger
   - Include business rule documentation

4. **Error Handling**
   - Use proper exception handling
   - Return appropriate error messages
   - Log errors with context

5. **Multi-tenancy**
   - Ensure proper tenant isolation
   - Use correct DbContext
   - Test with multiple tenants 