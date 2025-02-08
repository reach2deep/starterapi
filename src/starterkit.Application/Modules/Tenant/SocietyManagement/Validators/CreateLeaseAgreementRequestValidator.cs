using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class CreateLeaseAgreementRequestValidator : AbstractValidator<CreateLeaseAgreementRequest>
    {
        public CreateLeaseAgreementRequestValidator()
        {
            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit ID is required");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required");

            RuleFor(x => x.TenantId)
                .NotEmpty().WithMessage("Tenant ID is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(startDate => startDate.Date >= System.DateTime.UtcNow.Date)
                .WithMessage("Start date must be today or in the future");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .Must((request, endDate) => endDate > request.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.RentAmount)
                .NotEmpty().WithMessage("Rent amount is required")
                .GreaterThan(0).WithMessage("Rent amount must be greater than 0");

            RuleFor(x => x.SecurityDeposit)
                .NotEmpty().WithMessage("Security deposit is required")
                .GreaterThanOrEqualTo(0).WithMessage("Security deposit must be greater than or equal to 0");

            RuleFor(x => x.PaymentFrequency)
                .NotEmpty().WithMessage("Payment frequency is required")
                .MaximumLength(20).WithMessage("Payment frequency cannot exceed 20 characters")
                .Must(frequency => new[] { "Monthly", "Quarterly", "Yearly" }.Contains(frequency))
                .WithMessage("Payment frequency must be Monthly, Quarterly, or Yearly");

            RuleFor(x => x.NoticePeriodDays)
                .NotEmpty().WithMessage("Notice period is required")
                .InclusiveBetween(0, 365).WithMessage("Notice period must be between 0 and 365 days");
        }
    }
} 