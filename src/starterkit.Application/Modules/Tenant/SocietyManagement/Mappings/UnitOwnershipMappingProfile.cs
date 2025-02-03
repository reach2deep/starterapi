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
            CreateMap<UpdateUnitOwnershipRequest, UnitOwnership>();
            CreateMap<UnitOwnership, UnitOwnershipResponse>();
        }
    }
} 