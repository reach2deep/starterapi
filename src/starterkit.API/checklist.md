# Clean Architecture Implementation Checklist (Module-Based)

## 1. Core (Domain) Layer Checklist
### Domain Entities and Models
- [ ] Entity is placed in appropriate module (`Modules/[ModuleName]/Entities`)
- [ ] Entity inherits from correct base class (`BaseEntity` or `AuditableEntity`)
- [ ] No dependency on any outer layer (Application, Infrastructure)
- [ ] No reference to DTOs or external libraries
- [ ] Contains only domain properties and business rules
- [ ] Properties have appropriate access modifiers (private setters where needed)

### Repository Interfaces
- [ ] Interface placed in `Core/Modules/[ModuleName]/Interfaces/Repositories`
- [ ] Defines only essential data access methods needed by domain
- [ ] Methods use domain entities as parameters/return types
- [ ] No reference to DTOs or infrastructure concerns
- [ ] Named with 'I' prefix (e.g., `IUserRepository`)

### Enums
- [ ] Placed in `Core/Modules/[ModuleName]/Enums`
- [ ] Represents domain concept specific to the module
- [ ] No dependency on outer layers

## 2. Application Layer Checklist
### Module Structure
- [ ] Module folder created under `Application/Modules/[Global|Tenant]/[ModuleName]`
- [ ] Consistent internal structure for each module:
  - DTOs/
  - Services/
  - Interfaces/
  - Mappings/
  - Validators/

### DTOs
- [ ] Placed in `Modules/[ModuleName]/DTOs`
- [ ] Separate DTOs for request and response
- [ ] No circular references
- [ ] Contains only necessary properties for use case
- [ ] Follows naming convention (`[Entity][Action]Request/Response`)

### Service Interfaces
- [ ] Placed in `Modules/[ModuleName]/Interfaces`
- [ ] Methods represent specific use cases
- [ ] Uses DTOs for input/output
- [ ] No infrastructure dependencies
- [ ] Named with 'I' prefix (e.g., `IAuthService`)
- [ ] Methods follow consistent naming patterns
- [ ] Clear documentation for each service method

### Service Implementations
- [ ] Placed in `Modules/[ModuleName]/Services`
- [ ] Implements corresponding interface
- [ ] Uses only dependencies declared in constructor
- [ ] No direct infrastructure dependencies (uses interfaces)
- [ ] Contains proper exception handling
- [ ] Includes logging where appropriate
- [ ] Business logic properly encapsulated
- [ ] Follows Single Responsibility Principle

### Validators
- [ ] Placed in `Modules/[ModuleName]/Validators`
- [ ] One validator per request DTO
- [ ] Comprehensive validation rules
- [ ] No domain logic (validation only)
- [ ] Named appropriately (`[Dto]Validator`)
- [ ] Reusable validation rules extracted when appropriate

### Mappings
- [ ] Placed in `Modules/[ModuleName]/Mappings`
- [ ] One profile per module
- [ ] All DTOs have corresponding mapping configurations
- [ ] No complex logic in mappings
- [ ] Proper handling of nested objects
- [ ] Consistent mapping conventions across modules

## 3. Infrastructure Layer Checklist
### Repository Implementations
- [ ] Placed in `Infrastructure/Repositories/[ModuleName]`
- [ ] Implements interface from Core layer
- [ ] Uses appropriate DbContext
- [ ] Proper error handling
- [ ] No domain logic
- [ ] Follows repository pattern best practices

### Database Configurations
- [ ] Entity configurations in `Infrastructure/Data/Configurations/[ModuleName]`
- [ ] All entity properties properly configured
- [ ] Relationships clearly defined
- [ ] Proper database constraints
- [ ] Indexes defined where needed

### Infrastructure Services
- [ ] Module-specific services in `Infrastructure/Services/[ModuleName]`
- [ ] Implements application layer interface
- [ ] External service concerns isolated
- [ ] Proper error handling
- [ ] Configuration properly injected
- [ ] No domain logic

## 4. API Layer Checklist
### Controllers
- [ ] Placed in `Controllers/Modules/V[n]/[ModuleName]`
- [ ] Inherits from `BaseApiController`
- [ ] Uses only Application layer services
- [ ] Returns appropriate HTTP status codes
- [ ] Proper route attributes
- [ ] Proper authorization attributes
- [ ] Uses DTOs from Application layer
- [ ] No business logic

### Middleware/Filters
- [ ] Properly registered in startup
- [ ] Single responsibility
- [ ] No domain logic
- [ ] Proper error handling

## 5. Cross-Cutting Concerns
### Dependency Injection
- [ ] Module services registered with appropriate lifetime
- [ ] Dependencies flow inward
- [ ] No circular dependencies
- [ ] Core layer has no external dependencies
- [ ] Each module's dependencies properly isolated

### Error Handling
- [ ] Global exception handling
- [ ] Module-specific error handling where needed
- [ ] Appropriate error messages
- [ ] Proper logging
- [ ] Security considerations in error responses

### Module Integration
- [ ] Clear boundaries between modules
- [ ] Proper cross-module communication (if needed)
- [ ] Shared components in appropriate location
- [ ] No unnecessary module coupling

## 6. Multi-Tenant Database Initialization Checklist
### Database Context Registration
- [ ] DbContext options properly registered with correct lifetime
- [ ] DbContextFactory properly handles tenant-specific parameters
- [ ] Connection strings properly configured in appsettings.json
- [ ] Tenant-specific connection strings handled correctly
- [ ] DbContext constructor parameters properly provided

### Tenant Database Initialization
- [ ] Database initializer properly registered in DI
- [ ] Migration runner properly configured
- [ ] Seed data properly handled
- [ ] Error handling for failed initialization
- [ ] Proper cleanup if initialization fails

### Dependency Injection for Multi-Tenancy
- [ ] All required services registered in correct order
- [ ] Service lifetimes properly scoped
- [ ] Tenant resolution properly handled
- [ ] Proper error handling for missing tenant context
- [ ] Circular dependencies avoided in tenant services

### Validation Checks
- [ ] Validators properly registered for all DTOs
- [ ] Validator assemblies correctly scanned
- [ ] Validation rules properly applied
- [ ] Custom validation messages defined
- [ ] Validation context properly handled

### Database Migration Checks
- [ ] Migration files properly created
- [ ] Migration properly handles new columns
- [ ] Rollback scenarios considered
- [ ] Data preservation handled in migrations
- [ ] Migration applied in correct order

## Final Verification
- [ ] Dependencies point inward
- [ ] No circular references
- [ ] Core layer completely independent
- [ ] Application layer depends only on Core
- [ ] Infrastructure implements interfaces from Core and Application
- [ ] API layer uses only Application layer services
- [ ] Feature properly integrated with multi-tenancy
- [ ] Module boundaries respected
- [ ] All tests passing
- [ ] Module documentation complete