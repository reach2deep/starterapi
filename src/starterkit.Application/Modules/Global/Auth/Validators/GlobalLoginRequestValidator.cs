using FluentValidation;
using starterkit.Application.DTOs.Global.Auth;

namespace starterkit.Application.Validators.Global
{
    public class GlobalLoginRequestValidator : AbstractValidator<GlobalLoginRequestDto>
    {
        public GlobalLoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
        }
    }
} 