# API Development Guide and Checklist

## Overview
This guide provides a structured approach to implementing new API features in the starterkit project following clean architecture principles. Each section includes a checklist with scoring criteria.

## 1. Core (Domain) Layer [25 points]
### Entity Implementation [15 points]
- [ ] Entity placed in correct module folder (`Core/Modules/[ModuleName]`) [3pts]
- [ ] Inherits from appropriate base class (e.g., `BaseEntity`) [3pts]
- [ ] Properties properly defined with correct types and access modifiers [4pts]
- [ ] No dependencies on outer layers [3pts]
- [ ] Proper relationships defined with other entities [2pts]

### Repository Interface [10 points]
- [ ] Interface in correct location (`Core/Modules/[ModuleName]/Interfaces/Repositories`) [2pts]
- [ ] Essential CRUD methods defined [2pts]
- [ ] Method signatures use domain entities (not DTOs) [2pts]
- [ ] Proper naming convention (`I[Entity]Repository`) [2pts]
- [ ] Async methods with proper return types [2pts]

## 2. Application Layer [30 points]
### DTOs [10 points]
- [ ] Separate DTOs for different operations (Create/Update/Response) [3pts]
- [ ] Properties match API requirements (not just entity mirror) [3pts]
- [ ] Proper data annotations/validation attributes [2pts]
- [ ] Nested DTOs handled appropriately [2pts]

### Service Interface & Implementation [10 points]
- [ ] Interface defines all required operations [2pts]
- [ ] Service implements all interface methods [2pts]
- [ ] Proper error handling with appropriate responses [2pts]
- [ ] Logging implemented for important operations [2pts]
- [ ] Proper use of dependency injection [2pts]

### Validators [5 points]
- [ ] Validator for each request DTO [2pts]
- [ ] Comprehensive validation rules [2pts]
- [ ] Custom validation messages [1pt]

### Mapping Profiles [5 points]
- [ ] All required mappings defined [2pts]
- [ ] Proper handling of nested objects [2pts]
- [ ] Custom value resolvers where needed [1pt]

## 3. Infrastructure Layer [25 points]
### Repository Implementation [15 points]
- [ ] Implements repository interface completely [3pts]
- [ ] Proper use of DbContext/ORM [3pts]
- [ ] Efficient queries (no N+1 problems) [3pts]
- [ ] Proper error handling [2pts]
- [ ] Implements soft delete where appropriate [2pts]
- [ ] Proper transaction handling [2pts]

### Dependency Injection [10 points]
- [ ] All services registered with appropriate lifetime [3pts]
- [ ] Repository registered with appropriate lifetime [3pts]
- [ ] No circular dependencies [2pts]
- [ ] Proper use of interfaces [2pts]

## 4. API Layer [20 points]
### Controller Implementation [15 points]
- [ ] Proper route attributes [3pts]
- [ ] Proper HTTP method attributes [2pts]
- [ ] Proper response types defined [2pts]
- [ ] Authentication/Authorization attributes [2pts]
- [ ] Proper model validation [2pts]
- [ ] Consistent response format [2pts]
- [ ] Proper status codes used [2pts]

### Error Handling [5 points]
- [ ] Global exception handling [2pts]
- [ ] Proper error responses [2pts]
- [ ] Logging of errors [1pt]

## Example Implementation Review
Using our Permission Management implementation as an example:

### Core Layer ✅ [23/25]
- Entity properly defined in Core layer
- Repository interface with all necessary methods
- Missing some XML documentation (-2)

### Application Layer ✅ [28/30]
- Complete DTO definitions
- Service interface and implementation
- Proper validation
- Missing some custom validation messages (-2)

### Infrastructure Layer ✅ [24/25]
- Complete repository implementation
- Proper DI registration
- Missing explicit transaction handling (-1)

### API Layer ✅ [20/20]
- Complete controller implementation
- Proper routing and HTTP methods
- Comprehensive response type definitions
- Proper authentication/authorization

Total Score: 97/100 ⭐⭐⭐⭐⭐

## Scoring Guide
- 90-100: Excellent ⭐⭐⭐⭐⭐
- 80-89: Good ⭐⭐⭐⭐
- 70-79: Satisfactory ⭐⭐⭐
- 60-69: Needs Improvement ⭐⭐
- <60: Requires Significant Revision ⭐

## Best Practices
1. Always implement interfaces before concrete classes
2. Use meaningful names for methods and properties
3. Keep controllers thin, business logic in services
4. Implement proper logging throughout
5. Use dependency injection
6. Implement proper validation
7. Use async/await consistently
8. Implement proper error handling
9. Follow REST conventions
10. Document your APIs

## Common Pitfalls to Avoid
1. ❌ Circular dependencies
2. ❌ Business logic in controllers
3. ❌ Direct entity exposure in APIs
4. ❌ Missing error handling
5. ❌ Missing validation
6. ❌ Improper use of async/await
7. ❌ Missing logging
8. ❌ Tight coupling between layers
9. ❌ Missing documentation
10. ❌ Inconsistent response formats 