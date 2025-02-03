using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class CreateUnitRequestValidator : AbstractValidator<CreateUnitRequest>
    {
        public CreateUnitRequestValidator()
        {
            RuleFor(x => x.FloorId)
                .NotEmpty().WithMessage("Floor ID is required");

            RuleFor(x => x.UnitNumber)
                .NotEmpty().WithMessage("Unit number is required")
                .MaximumLength(20).WithMessage("Unit number cannot exceed 20 characters")
                .Matches(@"^[\w\d-]+$").WithMessage("Unit number can only contain letters, numbers, and dashes");

            RuleFor(x => x.SquareFeet)
                .InclusiveBetween(100, 10000).WithMessage("Square feet must be between 100 and 10000");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Unit type is required")
                .MaximumLength(50).WithMessage("Unit type cannot exceed 50 characters");
        }
    }
} 