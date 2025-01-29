

# Multi-Tenancy Architecture with Root Database Approach

Here's a detailed approach for implementing multi-tenancy with a root database strategy:

## Database Structure

### 1. Root Database (Global Database)
- Stores tenant configurations
- Basic user authentication info
- Tenant-user mappings
- Global configurations

### 2. Tenant Databases
- Separate database for each tenant
- Contains tenant-specific business data
- Follows same schema across all tenants

## Key Components

### 1. Root Database Tables
```
- Tenants
  - TenantId
  - Name
  - ConnectionString
  - Status
  - Settings
  - CreatedAt

- GlobalUsers
  - UserId
  - Email
  - PasswordHash
  - Status

- TenantUserMappings
  - Id
  - TenantId
  - UserId
  - Role
  - Status
```

## Flow Architecture

1. **Authentication Flow**
   - User provides credentials
   - System checks GlobalUsers table in Root DB
   - If authenticated:
     - Retrieves associated tenants
     - User selects tenant (if multiple)
     - System generates JWT with tenant context

2. **Request Pipeline**
   - Middleware extracts tenant info from JWT
   - Resolves tenant's database connection
   - Initializes tenant-specific DbContext
   - Routes request to tenant-specific services

3. **Tenant Resolution Strategy**
   ```
   Request → 
   TenantMiddleware → 
   TenantResolver → 
   TenantDbContext →
   Business Logic
   ```

## Service Architecture

### 1. Global Services (Root Level)
- Authentication Service
- Tenant Management Service
- User-Tenant Mapping Service

### 2. Tenant Services (Tenant Level)
- Business Logic Services
- Data Access Services
- Tenant-specific Operations

## Suggested Project Structure

```
    starterkit/
├── src/
│   ├── starterkit.API/
│   │   ├── Controllers/
│   │   │   ├── V1/
│   │   │   │   ├── Global/
│   │   │   │   │   ├── AuthController.cs           # Global auth & tenant selection
│   │   │   │   │   └── TenantsController.cs        # Tenant CRUD operations
│   │   │   │   └── Tenant/
│   │   │   │       ├── UsersController.cs          # Tenant user management
│   │   │   │       └── ProfileController.cs        # User profile management
│   │   │   └── BaseApiController.cs
│   │   ├── Middlewares/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── TenantMiddleware.cs
│   │   ├── Filters/
│   │   │   └── TenantAuthorizationFilter.cs
│   │   └── Program.cs
│   │
│   ├── starterkit.Core/
│   │   ├── Entities/
│   │   │   ├── Common/
│   │   │   │   ├── BaseEntity.cs
│   │   │   │   └── AuditableEntity.cs
│   │   │   ├── Global/
│   │   │   │   ├── Tenant.cs
│   │   │   │   ├── GlobalUser.cs
│   │   │   │   └── TenantUserMapping.cs
│   │   │   └── Tenant/
│   │   │       ├── User.cs
│   │   │       ├── Role.cs
│   │   │       ├── UserRole.cs
│   │   │       └── RefreshToken.cs
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── Global/
│   │   │   │   │   ├── ITenantRepository.cs
│   │   │   │   │   └── IGlobalUserRepository.cs
│   │   │   │   └── Tenant/
│   │   │   │       ├── IUserRepository.cs
│   │   │   │       └── IRefreshTokenRepository.cs
│   │   │   └── Services/
│   │   │       ├── Global/
│   │   │       │   ├── IGlobalAuthService.cs
│   │   │       │   └── ITenantService.cs
│   │   │       └── Tenant/
│   │   │           ├── IAuthService.cs
│   │   │           └── IUserService.cs
│   │   └── Enums/
│   │       ├── UserStatus.cs
│   │       └── TenantStatus.cs
│   │
│   ├── starterkit.Application/
│   │   ├── DTOs/
│   │   │   ├── Global/
│   │   │   │   ├── Auth/
│   │   │   │   │   ├── GlobalLoginRequestDto.cs
│   │   │   │   │   ├── GlobalLoginResponseDto.cs
│   │   │   │   │   └── TenantSelectionDto.cs
│   │   │   │   └── Tenant/
│   │   │   │       ├── TenantDto.cs
│   │   │   │       ├── CreateTenantDto.cs
│   │   │   │       └── UpdateTenantDto.cs
│   │   │   └── Tenant/
│   │   │       ├── Auth/
│   │   │       │   ├── LoginRequestDto.cs
│   │   │       │   └── LoginResponseDto.cs
│   │   │       └── Users/
│   │   │           ├── UserDto.cs
│   │   │           ├── CreateUserDto.cs
│   │   │           └── UpdateUserDto.cs
│   │   ├── Services/
│   │   │   ├── Global/
│   │   │   │   ├── GlobalAuthService.cs
│   │   │   │   └── TenantService.cs
│   │   │   └── Tenant/
│   │   │       ├── AuthService.cs
│   │   │       └── UserService.cs
│   │   ├── Validators/
│   │   │   ├── Global/
│   │   │   │   ├── CreateTenantValidator.cs
│   │   │   │   └── GlobalLoginValidator.cs
│   │   │   └── Tenant/
│   │   │       ├── CreateUserValidator.cs
│   │   │       └── UpdateUserValidator.cs
│   │   └── Mappings/
│   │       ├── GlobalMappingProfile.cs
│   │       └── TenantMappingProfile.cs
│   │
│   ├── starterkit.Infrastructure/
│   │   ├── Data/
│   │   │   ├── RootDb/
│   │   │   │   ├── RootDbContext.cs
│   │   │   │   ├── Configurations/
│   │   │   │   │   ├── TenantConfiguration.cs
│   │   │   │   │   ├── GlobalUserConfiguration.cs
│   │   │   │   │   └── TenantUserMappingConfiguration.cs
│   │   │   │   └── Repositories/
│   │   │   │       ├── TenantRepository.cs
│   │   │   │       └── GlobalUserRepository.cs
│   │   │   └── TenantDb/
│   │   │       ├── TenantDbContext.cs
│   │   │       ├── Configurations/
│   │   │       │   ├── UserConfiguration.cs
│   │   │       │   ├── RoleConfiguration.cs
│   │   │       │   └── RefreshTokenConfiguration.cs
│   │   │       └── Repositories/
│   │   │           ├── UserRepository.cs
│   │   │           └── RefreshTokenRepository.cs
│   │   ├── Services/
│   │   │   ├── TenantResolver.cs
│   │   │   ├── ConnectionStringResolver.cs
│   │   │   ├── JwtTokenService.cs
│   │   │   └── PasswordHashService.cs
│   │   └── MultiTenancy/
│   │       ├── Models/
│   │       │   ├── TenantInfo.cs
│   │       │   └── TenantConnectionConfig.cs
│   │       ├── Stores/
│   │       │   ├── ITenantStore.cs
│   │       │   └── CachedTenantStore.cs
│   │       └── Options/
│   │           └── MultiTenancyOptions.cs
│   │
│   └── starterkit.Shared/
│       ├── Constants/
│       │   ├── AuthConstants.cs
│       │   ├── TenantConstants.cs
│       │   └── ClaimConstants.cs
│       ├── Extensions/
│       │   ├── ClaimsPrincipalExtensions.cs
│       │   ├── TenantExtensions.cs
│       │   └── StringExtensions.cs
│       └── Helpers/
│           ├── ConnectionStringBuilder.cs
│           └── TenantHelper.cs
│
└── tests/
    ├── starterkit.UnitTests/
    │   ├── Global/
    │   │   ├── TenantServiceTests.cs
    │   │   └── GlobalAuthServiceTests.cs
    │   └── Tenant/
    │       ├── AuthServiceTests.cs
    │       └── UserServiceTests.cs
    └── starterkit.IntegrationTests/
        ├── Global/
        │   └── TenantManagementTests.cs
        └── Tenant/
            └── UserManagementTests.cs  
```

## Key Implementation Considerations

1. **Tenant Resolution**
   - HTTP Header
   - JWT Claim
   - Subdomain
   - Custom Route

2. **Connection String Management**
   - Encrypted storage
   - Dynamic resolution
   - Connection pooling
   

3. **Data Isolation**
   - Separate databases (recommended)
   - Schema-based separation
   - Row-level filtering

4. **Performance Optimization**
   - Connection pooling
   - Tenant context caching
   - Query optimization

5. **Security Considerations**
   - Tenant data isolation
   - Cross-tenant access prevention
   - Connection string encryption
   - Role-based access per tenant

## Middleware Pipeline

```
1. Authentication
2. Tenant Resolution
3. Database Context Resolution
4. Request Processing
5. Response
```

## Error Handling

1. **Tenant-specific Errors**
   - Invalid tenant
   - Inactive tenant
   - Database connection issues

2. **Cross-tenant Errors**
   - Authorization failures
   - Invalid tenant switching
   - Data isolation violations


So for testing, you can use these credentials:
F
or Root Admin:
Email: rootadmin@example.com
Password: Admin@123

For Tenant Admins:
Alpha Tenant Admin:
Email: admin@alpha.com
Password: Admin@123

Beta Tenant Admin:
Email: admin@beta.com
Password: Admin@123


cd src/starterkit.Infrastructure

Root migrations
dotnet ef migrations add InitialRootSchema -c RootDbContext -p src/starterkit.Infrastructure -s src/starterkit.API -o Data/RootDb/Migrations

Tenant migrations
dotnet ef migrations add InitialTenantSchema -c TenantDbContext -p src/starterkit.Infrastructure -s src/starterkit.API -o Data/TenantDb/Migrations

  # Make sure you're in the Infrastructure project directory
cd src/StarterApi.Infrastructure

# Create Root DB migration
dotnet ef migrations add InitialRootSchema --context RootDbContext --output-dir Persistence/Migrations/RootDb

# Create Tenant DB migration
dotnet ef migrations add InitialTenantSchema --context TenantDbContext --output-dir Persistence/Migrations/TenantDb
