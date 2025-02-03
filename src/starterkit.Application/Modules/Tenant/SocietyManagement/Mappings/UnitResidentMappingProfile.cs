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
            CreateMap<UpdateUnitResidentRequest, UnitResident>();
            CreateMap<UnitResident, UnitResidentResponse>();
        }
    }
} 