using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.DTOs;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.starterkit.Application.Persistence;
using starterkit.starterkit.Core.Enums;
using starterkit.starterkit.Core.Modules.Common;
using starterkit.starterkit.Core.Modules.Global;

namespace starterkit.starterkit.Application.Modules.Global.TenantManagement.Services
{
    public class TenantService : ITenantService
    {
        private readonly IRootDbContext _context;
        private readonly ITenantDatabaseInitializer _databaseInitializer;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateTenantRequestDto> _createValidator;
        private readonly IValidator<UpdateTenantRequestDto> _updateValidator;

        public TenantService(
            IRootDbContext context,
            ITenantDatabaseInitializer databaseInitializer,
            IMapper mapper,
            IValidator<CreateTenantRequestDto> createValidator,
            IValidator<UpdateTenantRequestDto> updateValidator)
        {
            _context = context;
            _databaseInitializer = databaseInitializer;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
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

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            // Initialize the tenant database
            await _databaseInitializer.InitializeTenantDatabaseAsync(tenant);

            return _mapper.Map<TenantResponseDto>(tenant);
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