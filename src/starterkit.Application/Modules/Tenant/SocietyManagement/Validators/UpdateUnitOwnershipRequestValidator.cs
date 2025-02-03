using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class UpdateUnitOwnershipRequestValidator : AbstractValidator<UpdateUnitOwnershipRequest>
    {
        public UpdateUnitOwnershipRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required");

            RuleFor(x => x.UnitId)
                .NotEmpty().WithMessage("Unit ID is required");

            RuleFor(x => x.OwnerId)
                .NotEmpty().WithMessage("Owner ID is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required")
                .Must(date => date <= DateTime.UtcNow).WithMessage("Start date cannot be in the future");

            RuleFor(x => x.EndDate)
                .Must((request, endDate) => !endDate.HasValue || endDate.Value > request.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }
} 