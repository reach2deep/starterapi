using AutoMapper;
using starterkit.starterkit.Application.Modules.Global.Auth.DTOs;
using starterkit.starterkit.Core.Modules.Global;


namespace starterkit.starterkit.Application.Modules.Global.Auth.Mappings
{
    public class GlobalMappingProfile : Profile
    {
        public GlobalMappingProfile()
        {
            CreateMap<TenantUserMapping, TenantAccessDto>()
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<GlobalUser, GlobalLoginResponseDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.BaseToken, opt => opt.Ignore())
                .ForMember(dest => dest.AvailableTenants, opt => opt.Ignore());
        }
    }
}