using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class CreateFloorRequestValidator : AbstractValidator<CreateFloorRequest>
    {
        public CreateFloorRequestValidator()
        {
            RuleFor(x => x.BlockId)
                .NotEmpty().WithMessage("Block ID is required");

            RuleFor(x => x.FloorNumber)
                .InclusiveBetween(0, 200).WithMessage("Floor number must be between 0 and 200");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Floor name is required")
                .MaximumLength(50).WithMessage("Floor name cannot exceed 50 characters");

            RuleFor(x => x.TotalUnits)
                .InclusiveBetween(1, 50).WithMessage("Total units must be between 1 and 50");
        }
    }
} 