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
                .ForMember(dest => dest.RentPayments, opt => opt.MapFrom(src => src.RentPayments))
                // Calculate IsExpiringSoon (within 30 days)
                .ForMember(dest => dest.IsExpiringSoon, opt => opt.MapFrom(src => 
                    src.EndDate.Date > DateTime.UtcNow.Date && 
                    (src.EndDate.Date - DateTime.UtcNow.Date).Days <= 30))
                // Calculate DaysUntilExpiry
                .ForMember(dest => dest.DaysUntilExpiry, opt => opt.MapFrom(src => 
                    src.EndDate.Date > DateTime.UtcNow.Date ? 
                    (src.EndDate.Date - DateTime.UtcNow.Date).Days : 0))
                // Calculate TotalPaidAmount
                .ForMember(dest => dest.TotalPaidAmount, opt => opt.MapFrom(src => 
                    src.RentPayments != null ? 
                    src.RentPayments.Where(p => p.Status == "Paid").Sum(p => p.Amount) : 0))
                // Calculate PendingAmount
                .ForMember(dest => dest.PendingAmount, opt => opt.MapFrom(src => 
                    src.RentPayments != null ? 
                    src.RentPayments.Where(p => p.Status == "Pending" || p.Status == "Overdue").Sum(p => p.Amount) : 0))
                // Get LastPaymentDate
                .ForMember(dest => dest.LastPaymentDate, opt => opt.MapFrom(src => 
                    src.RentPayments != null && src.RentPayments.Any() 
                        ? src.RentPayments.OrderByDescending(p => p.PaidDate)
                            .Select(p => p.PaidDate)
                            .FirstOrDefault()
                        : (DateTime?)null))
                // Get LastPaymentStatus
                .ForMember(dest => dest.LastPaymentStatus, opt => opt.MapFrom(src => 
                    src.RentPayments != null && src.RentPayments.Any() 
                        ? src.RentPayments.OrderByDescending(p => p.PaidDate)
                            .Select(p => p.Status)
                            .FirstOrDefault()
                        : null));

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
                .ForMember(dest => dest.LeaseAgreement, opt => opt.MapFrom(src => 
                    src.LeaseAgreement != null 
                        ? new LeaseAgreementBasicInfo
                        {
                            Id = src.LeaseAgreement.Id,
                            UnitNumber = src.LeaseAgreement.Unit != null ? src.LeaseAgreement.Unit.UnitNumber : null,
                            TenantName = src.LeaseAgreement.Tenant != null ? src.LeaseAgreement.Tenant.FullName : null
                        } 
                        : null));
        }
    }
} 