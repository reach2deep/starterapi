using AutoMapper;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Mappings
{
    /// <summary>
    /// AutoMapper profile for Unit Management
    /// </summary>
    public class UnitMappingProfile : Profile
    {
        public UnitMappingProfile()
        {
            // Unit mappings
            CreateMap<CreateUnitRequest, Unit>();
            CreateMap<UpdateUnitRequest, Unit>();
            CreateMap<Unit, UnitResponse>()
                .ForMember(dest => dest.FloorName, opt => opt.MapFrom(src => src.Floor.Name))
                .ForMember(dest => dest.BlockId, opt => opt.MapFrom(src => src.Floor.BlockId))
                .ForMember(dest => dest.BlockName, opt => opt.MapFrom(src => src.Floor.Block.Name));
        }
    }
} 