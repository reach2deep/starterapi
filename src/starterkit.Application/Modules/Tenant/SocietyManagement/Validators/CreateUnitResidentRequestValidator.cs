using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class CreateUnitResidentRequestValidator : AbstractValidator<CreateUnitResidentRequest>
    {
        public CreateUnitResidentRequestValidator()
        {
            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit ID is required");

            RuleFor(x => x.ResidentId)
                .NotEmpty().WithMessage("Resident ID is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(date => date <= DateTime.UtcNow).WithMessage("Start date cannot be in the future");

            RuleFor(x => x.EndDate)
                .Must((request, endDate) => !endDate.HasValue || endDate.Value > request.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.ResidencyType)
                .NotEmpty().WithMessage("Residency type is required")
                .MaximumLength(50).WithMessage("Residency type cannot exceed 50 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }
} 