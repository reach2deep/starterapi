# Multi-Tenant Application Technical Documentation

## 1. Architecture Overview

### 1.1 Project Structure
```
starterkit/
├── src/
│   ├── starterkit.API/         # API endpoints and configuration
│   ├── starterkit.Core/        # Domain entities and enums
│   ├── starterkit.Application/ # Application services and DTOs
│   └── starterkit.Infrastructure/ # Data access and implementations
```

### 1.2 Database Architecture
The application uses two types of databases:
1. **Root Database** (`starterkit_root`)
   - Stores global users, tenants, and tenant-user mappings
   - Manages tenant configurations and access control

2. **Tenant Databases** (e.g., `alpha_tenant`, `beta_tenant`)
   - Separate database for each tenant
   - Contains tenant-specific users, profiles, and data

## 2. Multi-Tenancy Implementation

### 2.1 Tenant Resolution Flow
1. **TenantMiddleware**
   ```csharp
   // Resolves tenant from:
   1. X-Tenant-ID header
   2. JWT token claim (tenant_id)
   3. Subdomain
   ```

2. **TenantResolver**
   - Implements `ITenantResolver`
   - Uses `ITenantStore` to validate and retrieve tenant information
   - Skips resolution for global endpoints (`/api/v1/global/*`)

3. **CachedTenantStore**
   - Implements `ITenantStore`
   - Caches tenant information for 10 minutes
   - Looks up tenants by:
     - ID
     - Name
     - Database name

### 2.2 Database Connection Management
```csharp
// Scoped factory for tenant database contexts
builder.Services.AddScoped<Func<string, TenantDbContext>>(serviceProvider => databaseName =>
{
    var tenant = serviceProvider.GetService<ITenantStore>()?.GetTenantAsync(databaseName).Result;
    var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
    optionsBuilder.UseSqlServer(tenant.ConnectionString);
    return new TenantDbContext(optionsBuilder.Options, databaseName);
});
```

## 3. Authentication Flow

### 3.1 Global Authentication
1. User logs in with email/password to `/api/v1/global/auth/login`
2. `GlobalAuthService` validates credentials
3. Returns:
   - Base token
   - Available tenants
   - User information

### 3.2 Tenant Selection
1. User selects tenant with base token
2. `GlobalAuthService.SelectTenantAsync`:
   - Validates base token
   - Verifies tenant access
   - Generates tenant-specific JWT token

### 3.3 Tenant Access
- JWT token contains:
  ```json
  {
    "nameid": "user_id",
    "email": "user@example.com",
    "tenant_id": "tenant_guid",
    "role": "Admin"
  }
  ```

## 4. Database Seeding Process

### 4.1 Root Database Seeding (`RootDbSeeder`)
1. **Root Admin Creation**
   ```csharp
   var rootAdmin = new GlobalUser
   {
       Email = "rootadmin@example.com",
       UserType = UserType.RootAdmin,
       Status = UserStatus.Active
   };
   ```

2. **Tenant Creation**
   ```csharp
   var tenants = new[]
   {
       new Tenant { Name = "Alpha", DatabaseName = "alpha_tenant" },
       new Tenant { Name = "Beta", DatabaseName = "beta_tenant" }
   };
   ```

3. **Tenant-User Mappings**
   - Maps root admin to all tenants with Admin role

### 4.2 Tenant Database Seeding (`TenantDbSeeder`)
1. **User Synchronization**
   - Creates tenant user records for:
     - Root admin (same ID as global)
     - Tenant admin (same ID as global)

2. **Profile Creation**
   - Creates user profiles for each user

3. **Sample Data**
   - Creates sample addresses
   - Links addresses to user profiles

### 4.3 Database Initialization Flow
```csharp
// In Program.cs
1. Seed root database
2. Get all tenants
3. For each tenant:
   - Initialize tenant database
   - Apply migrations
   - Run tenant seeder
```

## 5. Entity Relationships

### 5.1 Root Database
```
GlobalUser 1 ─┬─── * TenantUserMapping * ──── 1 Tenant
              │
              └─── (Same ID) ──── User (in tenant DB)
```

### 5.2 Tenant Database
```
User 1 ──── 1 UserProfile ? ──── 1 Address
```

## 6. Key Features

### 6.1 ID Consistency
- Same user ID across root and tenant databases
- Enables seamless user tracking and authentication

### 6.2 Caching
- Tenant information cached for performance
- Cache duration: 10 minutes

### 6.3 Database Isolation
- Each tenant has separate database
- Complete data isolation between tenants

### 6.4 Role-Based Access
- Global roles (RootAdmin, TenantAdmin)
- Tenant-specific roles (Admin, User)

## 7. Security Considerations

### 7.1 Authentication
- JWT tokens with tenant context
- Refresh token rotation
- Token validation and expiration

### 7.2 Data Isolation
- Separate databases per tenant
- Tenant context validation in middleware
- No cross-tenant data access

### 7.3 Password Security
- BCrypt password hashing
- Configurable password policies

## 8. Configuration

### 8.1 Connection Strings
```json
{
  "ConnectionStrings": {
    "RootConnection": "Server=localhost;Database=starterkit_root;..."
  }
}
```

### 8.2 JWT Settings
```json
{
  "JWT": {
    "Secret": "...",
    "BaseTokenSecret": "...",
    "Issuer": "starterkit",
    "Audience": "starterkit-api"
  }
}
```

This documentation provides a comprehensive overview of the multi-tenant implementation. Let me know if you need more details about any specific aspect!
