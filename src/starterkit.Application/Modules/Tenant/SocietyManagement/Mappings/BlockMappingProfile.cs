using AutoMapper;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Mappings
{
    /// <summary>
    /// AutoMapper profile for Block Management
    /// </summary>
    public class BlockMappingProfile : Profile
    {
        public BlockMappingProfile()
        {
            // Block mappings
            CreateMap<CreateBlockRequest, Block>();
            CreateMap<UpdateBlockRequest, Block>();
            CreateMap<Block, BlockResponse>();
        }
    }
} 