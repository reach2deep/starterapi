using AutoMapper;
using starterkit.Application.Modules.Global.TenantManagement.DTOs;
using starterkit.Core.Modules.Global;

namespace starterkit.Application.Modules.Global.TenantManagement.Mappings
{
    public class TenantMappingProfile : Profile
    {
        public TenantMappingProfile()
        {
            CreateMap<CreateTenantRequestDto, Core.Modules.Global.Tenant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateTenantRequestDto, Core.Modules.Global.Tenant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DatabaseName, opt => opt.Ignore())
                .ForMember(dest => dest.ConnectionString, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Core.Modules.Global.Tenant, TenantResponseDto>();
        }
    }
} 