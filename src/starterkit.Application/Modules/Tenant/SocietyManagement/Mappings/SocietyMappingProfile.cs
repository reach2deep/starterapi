using AutoMapper;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;
using starterkit.Core.Modules.Tenant;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Mappings
{
    /// <summary>
    /// AutoMapper profile for Society Management module
    /// </summary>
    public class SocietyMappingProfile : Profile
    {
        public SocietyMappingProfile()
        {
            // Address mappings
            CreateMap<AddressDto, Address>();
            CreateMap<Address, AddressDto>();

            // Society mappings
            CreateMap<CreateSocietyRequest, Society>();
            CreateMap<UpdateSocietyRequest, Society>();
            CreateMap<Society, SocietyResponse>();
        }
    }
} 