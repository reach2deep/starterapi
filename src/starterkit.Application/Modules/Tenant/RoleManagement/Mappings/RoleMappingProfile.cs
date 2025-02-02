using AutoMapper;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;
using starterkit.Core.Modules.Tenant;

namespace starterkit.Application.Modules.Tenant.RoleManagement.Mappings
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleResponse>();
            CreateMap<Permission, PermissionResponse>();
            
            CreateMap<CreateRoleRequest, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());

            CreateMap<UpdateRoleRequest, Role>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
                .ForMember(dest => dest.RolePermissions, opt => opt.Ignore());
        }
    }
} 