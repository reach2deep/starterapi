using FluentValidation;

using starterkit.Application.Modules.Global.Auth.DTOs;

namespace starterkit.Application.Modules.Global.Auth.Validators
{
    public class TenantSelectionRequestValidator : AbstractValidator<TenantSelectionRequestDto>
    {
        public TenantSelectionRequestValidator()
        {
            RuleFor(x => x.BaseToken)
                .NotEmpty().WithMessage("Base token is required");

            RuleFor(x => x.TenantId)
                .NotEmpty().WithMessage("Tenant ID is required");
        }
    }
}