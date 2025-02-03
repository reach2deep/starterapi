using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class UpdateBlockRequestValidator : AbstractValidator<UpdateBlockRequest>
    {
        public UpdateBlockRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Block ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Block name is required")
                .Length(1, 50).WithMessage("Block name must be between 1 and 50 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.TotalFloors)
                .InclusiveBetween(1, 200).WithMessage("Total floors must be between 1 and 200");
        }
    }
} 