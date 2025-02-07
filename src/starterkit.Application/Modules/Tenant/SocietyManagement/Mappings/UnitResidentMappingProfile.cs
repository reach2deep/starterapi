using AutoMapper;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Mappings
{
    /// <summary>
    /// AutoMapper profile for Unit Resident Management
    /// </summary>
    public class UnitResidentMappingProfile : Profile
    {
        public UnitResidentMappingProfile()
        {
            // Unit Resident mappings
            CreateMap<CreateUnitResidentRequest, UnitResident>();
            
            CreateMap<UpdateUnitResidentRequest, UnitResident>()
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.Resident, opt => opt.Ignore());
            
            CreateMap<UnitResident, UpdateUnitResidentRequest>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.UnitNumber))
                .ForMember(dest => dest.ResidentName, opt => opt.MapFrom(src => 
                    src.Resident.Profile != null 
                        ? $"{src.Resident.Profile.FirstName} {src.Resident.Profile.LastName}"
                        : src.Resident.FullName ?? "Unknown"));
            
            CreateMap<UnitResident, UnitResidentResponse>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.UnitNumber))
                .ForMember(dest => dest.ResidentName, opt => opt.MapFrom(src => 
                    src.Resident.Profile != null 
                        ? $"{src.Resident.Profile.FirstName} {src.Resident.Profile.LastName}"
                        : src.Resident.FullName ?? "Unknown"));
        }
    }
} 