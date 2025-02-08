using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class UpdateLeaseAgreementRequestValidator : AbstractValidator<UpdateLeaseAgreementRequest>
    {
        public UpdateLeaseAgreementRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .Must(endDate => endDate > System.DateTime.UtcNow)
                .WithMessage("End date must be in the future");

            RuleFor(x => x.RentAmount)
                .NotEmpty().WithMessage("Rent amount is required")
                .GreaterThan(0).WithMessage("Rent amount must be greater than 0");

            RuleFor(x => x.PaymentFrequency)
                .NotEmpty().WithMessage("Payment frequency is required")
                .MaximumLength(20).WithMessage("Payment frequency cannot exceed 20 characters")
                .Must(frequency => new[] { "Monthly", "Quarterly", "Yearly" }.Contains(frequency))
                .WithMessage("Payment frequency must be Monthly, Quarterly, or Yearly");

            RuleFor(x => x.NoticePeriodDays)
                .NotEmpty().WithMessage("Notice period is required")
                .InclusiveBetween(0, 365).WithMessage("Notice period must be between 0 and 365 days");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(20).WithMessage("Status cannot exceed 20 characters")
                .Must(status => new[] { "Active", "Expired", "Terminated" }.Contains(status))
                .WithMessage("Status must be Active, Expired, or Terminated");
        }
    }
} 