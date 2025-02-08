using AutoMapper;
using starterkit.Core.Modules.Tenant.SocietyManagement.Entities;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Mappings
{
    public class LeaseManagementMappingProfile : Profile
    {
        public LeaseManagementMappingProfile()
        {
            // LeaseAgreement mappings
            CreateMap<CreateLeaseAgreementRequest, LeaseAgreement>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active"))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Unit, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Tenant, opt => opt.Ignore())
                .ForMember(dest => dest.RentPayments, opt => opt.Ignore());

            CreateMap<LeaseAgreement, LeaseAgreementResponse>()
                .ForMember(dest => dest.UnitNumber, opt => opt.MapFrom(src => src.Unit != null ? src.Unit.UnitNumber : null))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : null))
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.FullName : null))
                .ForMember(dest => dest.RentPayments, opt => opt.MapFrom(src => src.RentPayments));

            // RentPayment mappings
            CreateMap<CreateRentPaymentRequest, RentPayment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Pending"))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.LeaseAgreement, opt => opt.Ignore());

            CreateMap<RentPayment, RentPaymentResponse>()
                .ForMember(dest => dest.LeaseAgreement, opt => opt.MapFrom(src => new LeaseAgreementBasicInfo
                {
                    Id = src.LeaseAgreement.Id,
                    UnitNumber = src.LeaseAgreement.Unit.UnitNumber,
                    TenantName = src.LeaseAgreement.Tenant.FullName
                }));
        }
    }
} 