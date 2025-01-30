using FluentValidation;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.DTOs;
using starterkit.starterkit.Core.Enums;

namespace starterkit.starterkit.Application.Modules.Global.TenantManagement.Validators
{
    public class UpdateTenantRequestValidator : AbstractValidator<UpdateTenantRequestDto>
    {
        public UpdateTenantRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("^[a-zA-Z0-9-_. ]+$")
                .WithMessage("Name can only contain letters, numbers, spaces, dots, dashes, and underscores");

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid tenant status");
        }
    }
} 