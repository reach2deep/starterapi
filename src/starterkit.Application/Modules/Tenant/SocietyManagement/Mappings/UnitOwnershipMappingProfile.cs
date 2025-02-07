using AutoMapper;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Mappings
{
    /// <summary>
    /// AutoMapper profile for Unit Ownership Management
    /// </summary>
    public class UnitOwnershipMappingProfile : Profile
    {
        public UnitOwnershipMappingProfile()
        {
            // Unit Ownership mappings
            CreateMap<CreateUnitOwnershipRequest, UnitOwnership>();
            
            CreateMap<UpdateUnitOwnershipRequest, UnitOwnership>()
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore());
            
            CreateMap<UnitOwnership, UpdateUnitOwnershipRequest>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.UnitNumber))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => 
                    src.Owner.Profile != null 
                        ? $"{src.Owner.Profile.FirstName} {src.Owner.Profile.LastName}"
                        : src.Owner.FullName ?? "Unknown"));
            
            CreateMap<UnitOwnership, UnitOwnershipResponse>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.UnitNumber))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => 
                    src.Owner.Profile != null 
                        ? $"{src.Owner.Profile.FirstName} {src.Owner.Profile.LastName}"
                        : src.Owner.FullName ?? "Unknown"));
        }
    }
} 