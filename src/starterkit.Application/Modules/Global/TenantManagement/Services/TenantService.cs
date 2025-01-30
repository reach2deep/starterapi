using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using starterkit.Application.Modules.Global.TenantManagement.DTOs;
using starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.Application.Persistence;
using starterkit.Core.Enums;
using starterkit.Core.Modules.Common;
using starterkit.Core.Modules.Global;

namespace starterkit.Application.Modules.Global.TenantManagement.Services
{
    public class TenantService : ITenantService
    {
        private readonly IRootDbContext _context;
        private readonly ITenantDatabaseInitializer _databaseInitializer;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTenantRequestDto> _createValidator;
        private readonly IValidator<UpdateTenantRequestDto> _updateValidator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantService(
            IRootDbContext context,
            ITenantDatabaseInitializer databaseInitializer,
            IMapper mapper,
            IValidator<CreateTenantRequestDto> createValidator,
            IValidator<UpdateTenantRequestDto> updateValidator,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _databaseInitializer = databaseInitializer;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TenantResponseDto> CreateTenantAsync(CreateTenantRequestDto request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            // Validate tenant name uniqueness
            if (await _context.Tenants.AnyAsync(t => t.Name == request.Name))
            {
                throw new InvalidOperationException($"Tenant with name {request.Name} already exists");
            }

            // Validate database name uniqueness
            if (await _context.Tenants.AnyAsync(t => t.DatabaseName == request.DatabaseName))
            {
                throw new InvalidOperationException($"Database name {request.DatabaseName} is already in use");
            }

            var tenant = _mapper.Map<Core.Modules.Global.Tenant>(request);
            tenant.Status = TenantStatus.Active;
            tenant.IsActive = true;
            tenant.CreatedAt = DateTime.UtcNow;

            // Get the current user (root admin) ID
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            tenant.CreatedBy = currentUserId.Value;

            try
            {
                // Save tenant
                _context.Tenants.Add(tenant);
                await _context.SaveChangesAsync();

                // Create tenant user mapping for root admin
                var tenantUserMapping = new TenantUserMapping
                {
                    TenantId = tenant.Id,
                    UserId = currentUserId.Value,
                    Role = "RootAdmin",
                    IsActive = true,
                    CreatedBy = currentUserId.Value,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TenantUserMappings.Add(tenantUserMapping);
                await _context.SaveChangesAsync();

                // Initialize the tenant database
                await _databaseInitializer.InitializeTenantDatabaseAsync(tenant);

                return _mapper.Map<TenantResponseDto>(tenant);
            }
            catch
            {
                // If anything fails, we should try to clean up
                if (tenant.Id != Guid.Empty)
                {
                    try
                    {
                        // Try to remove the tenant if it was created
                        _context.Tenants.Remove(tenant);
                        await _context.SaveChangesAsync();
                    }
                    catch
                    {
                        // Log cleanup failure but throw the original exception
                    }
                }
                throw;
            }
        }

        private Guid? GetCurrentUserId()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return userIdString != null ? Guid.Parse(userIdString) : null;
        }

        public async Task<TenantResponseDto> UpdateTenantAsync(Guid id, UpdateTenantRequestDto request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                throw new KeyNotFoundException($"Tenant with ID {id} not found");
            }

            // Check name uniqueness if name is being changed
            if (request.Name != tenant.Name && await _context.Tenants.AnyAsync(t => t.Name == request.Name))
            {
                throw new InvalidOperationException($"Tenant with name {request.Name} already exists");
            }

            _mapper.Map(request, tenant);
            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return _mapper.Map<TenantResponseDto>(tenant);
        }

        public async Task<bool> DeactivateTenantAsync(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                throw new KeyNotFoundException($"Tenant with ID {id} not found");
            }

            tenant.IsActive = false;
            tenant.Status = TenantStatus.Inactive;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResponse<TenantResponseDto>> GetTenantsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Tenants.AsNoTracking();
            
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<TenantResponseDto>>(items);
            return new PagedResponse<TenantResponseDto>(dtos, totalCount, pageNumber, pageSize);
        }

        public async Task<TenantResponseDto> GetTenantByIdAsync(Guid id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
            {
                throw new KeyNotFoundException($"Tenant with ID {id} not found");
            }

            return _mapper.Map<TenantResponseDto>(tenant);
        }
    }
} 