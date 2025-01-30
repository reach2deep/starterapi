# Clean Architecture Implementation Checklist

## 1. Core (Domain) Layer Checklist
### Domain Entities
- [ ] Entity is placed in appropriate folder (`Global/` or `Tenant/`)
- [ ] Entity inherits from correct base class (`BaseEntity` or `AuditableEntity`)
- [ ] No dependency on any outer layer (Application, Infrastructure)
- [ ] No reference to DTOs or external libraries
- [ ] Contains only domain properties and business rules
- [ ] Properties have appropriate access modifiers (private setters where needed)

### Repository Interfaces
- [ ] Interface placed in `Core/Interfaces/Repositories/[Global|Tenant]`
- [ ] Defines only essential data access methods needed by domain
- [ ] Methods use domain entities as parameters/return types
- [ ] No reference to DTOs or infrastructure concerns
- [ ] Named with 'I' prefix (e.g., `IUserRepository`)

### Enums
- [ ] Placed in `Core/Enums`
- [ ] Represents domain concept
- [ ] No dependency on outer layers

## 2. Application Layer Checklist
### DTOs
- [ ] Placed in appropriate folder structure (`DTOs/[Global|Tenant]/[Feature]`)
- [ ] Separate DTOs for request and response
- [ ] No circular references
- [ ] Contains only necessary properties for use case
- [ ] Follows naming convention (`[Action][Entity]Dto`)

### Service Interfaces
- [ ] Placed in `Application/Interfaces/Services/[Global|Tenant]`
- [ ] Methods represent specific use cases
- [ ] Uses DTOs for input/output
- [ ] No infrastructure dependencies
- [ ] Named with 'I' prefix (e.g., `IAuthService`)

### Service Implementations
- [ ] Placed in `Application/Services/[Global|Tenant]`
- [ ] Implements corresponding interface
- [ ] Uses only dependencies declared in constructor
- [ ] No direct infrastructure dependencies (uses interfaces)
- [ ] Contains proper exception handling
- [ ] Includes logging where appropriate

### Validators
- [ ] Placed in `Application/Validators/[Global|Tenant]`
- [ ] One validator per DTO
- [ ] Comprehensive validation rules
- [ ] No domain logic (validation only)
- [ ] Named appropriately (`[Dto]Validator`)

### Mappings
- [ ] Placed in `Application/Mappings`
- [ ] Profiles separated by context (Global/Tenant)
- [ ] All DTOs have corresponding mapping configurations
- [ ] No complex logic in mappings

## 3. Infrastructure Layer Checklist
### Repository Implementations
- [ ] Placed in correct database context folder
- [ ] Implements interface from Core layer
- [ ] Uses appropriate DbContext
- [ ] Proper error handling
- [ ] No domain logic
- [ ] Follows repository pattern best practices

### Database Configurations
- [ ] Entity configurations in correct folder
- [ ] All entity properties properly configured
- [ ] Relationships clearly defined
- [ ] Proper database constraints
- [ ] Indexes defined where needed

### Infrastructure Services
- [ ] Implements application layer interface
- [ ] External service concerns isolated
- [ ] Proper error handling
- [ ] Configuration properly injected
- [ ] No domain logic

## 4. API Layer Checklist
### Controllers
- [ ] Placed in correct version and module folder
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
- [ ] All services registered with appropriate lifetime
- [ ] Dependencies flow inward
- [ ] No circular dependencies
- [ ] Core layer has no external dependencies

### Error Handling
- [ ] Global exception handling
- [ ] Appropriate error messages
- [ ] Proper logging
- [ ] Security considerations in error responses

## Final Verification
- [ ] Dependencies point inward
- [ ] No circular references
- [ ] Core layer completely independent
- [ ] Application layer depends only on Core
- [ ] Infrastructure implements interfaces from Core and Application
- [ ] API layer uses only Application layer services
- [ ] Feature properly integrated with multi-tenancy
- [ ] All tests passing