using FluentValidation;
using starterkit.Application.DTOs.Global.Auth;

namespace starterkit.Application.Validators.Global
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