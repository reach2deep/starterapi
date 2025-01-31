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

## Actual Project Structure

```
starterkit/
├── src/
│   ├── starterkit.API/
│   │   ├── Controllers/
│   │   │   ├── Modules/
│   │   │   │   ├── V1/
│   │   │   │   │   ├── Global/
│   │   │   │   │   │   └── Auth/
│   │   │   │   │   │       └── AuthController.cs
│   │   │   │   │   └── Tenant/
│   │   └── BaseApiController.cs
│   │   ├── DependencyInjection/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── Middlewares/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── TenantMiddleware.cs
│   │   └── Program.cs
│   │
│   ├── starterkit.Core/                            # Domain Layer
│   │   ├── Modules/
│   │   │   ├── Common/
│   │   │   │   ├── BaseEntity.cs
│   │   │   │   └── TenantInfo.cs
│   │   │   ├── Global/
│   │   │   │   └── Auth/
│   │   │   │       └── Entities/
│   │   │   └── Tenant/
│   │   └── Interfaces/
│   │       └── Repositories/
│   │
│   ├── starterkit.Application/                     # Application Layer
│   │   ├── Modules/
│   │   │   ├── Global/
│   │   │   │   └── Auth/
│   │   │   │       ├── DTOs/
│   │   │   │       ├── Interfaces/
│   │   │   │       ├── Services/
│   │   │   │       ├── Validators/
│   │   │   │       └── Mappings/
│   │   │   └── Tenant/
│   │   └── DependencyInjection/
│   │       └── ServiceCollectionExtensions.cs
│   │
│   ├── starterkit.Infrastructure/                  # Infrastructure Layer
│   │   ├── Persistence/
│   │   │   ├── RootDb/
│   │   │   │   ├── RootDbContext.cs
│   │   │   │   ├── RootDbContextFactory.cs
│   │   │   │   ├── Configurations/
│   │   │   │   └── Migrations/
│   │   │   └── TenantDb/
│   │   │       ├── TenantDbContext.cs
│   │   │       ├── TenantDbContextFactory.cs
│   │   │       ├── Configurations/
│   │   │       └── Migrations/
│   │   ├── Services/
│   │   │   ├── TenantResolver.cs
│   │   │   └── TenantDatabaseInitializer.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   └── Repositories/
│   │       ├── Global/
│   │       └── Tenant/
│   │
│   └── starterkit.Shared/                         # Shared Kernel
│       ├── Constants/
│       ├── Extensions/
│       └── Helpers/
│
└── tests/
    ├── starterkit.UnitTests/
    │   ├── Modules/
    │   │   ├── Global/
    │   │   └── Tenant/
    └── starterkit.IntegrationTests/
        ├── Modules/
        │   ├── Global/
        │   └── Tenant/
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


{
  "email": "rootadmin@example.com",
  "password": "Admin@123"
}

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
dotnet ef migrations add LoginActivity -c RootDbContext -p src/starterkit.Infrastructure -s src/starterkit.API -o Persistence/RootDb/Migrations

Tenant migrations
dotnet ef migrations add RolesSchema -c TenantDbContext -p src/starterkit.Infrastructure -s src/starterkit.API -o Persistence/TenantDb/Migrations

  # Make sure you're in the Infrastructure project directory
cd src/StarterApi.Infrastructure

# Create Root DB migration
dotnet ef migrations add InitialRootSchema --context RootDbContext --output-dir Persistence/Migrations/RootDb

# Create Tenant DB migration
dotnet ef migrations add InitialTenantSchema --context TenantDbContext --output-dir Persistence/Migrations/TenantDb
