using FluentValidation;
using starterkit.Core.DTOs.Tenant;

namespace starterkit.Application.Validators.Tenant
{
    public class UpdateUserProfileRequestValidator : AbstractValidator<UpdateUserProfileRequestDto>
    {
        public UpdateUserProfileRequestValidator()
        {
            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required")
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth cannot be in the future");

            RuleFor(x => x.ProfilePictureUrl)
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ProfilePictureUrl))
                .WithMessage("Profile picture URL must be a valid URL");

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address.StreetAddress)
                    .NotEmpty().WithMessage("Street address is required")
                    .MaximumLength(200).WithMessage("Street address cannot exceed 200 characters");

                RuleFor(x => x.Address.City)
                    .NotEmpty().WithMessage("City is required")
                    .MaximumLength(100).WithMessage("City cannot exceed 100 characters");

                RuleFor(x => x.Address.Country)
                    .NotEmpty().WithMessage("Country is required")
                    .MaximumLength(100).WithMessage("Country cannot exceed 100 characters");

                RuleFor(x => x.Address.PostalCode)
                    .NotEmpty().WithMessage("Postal code is required")
                    .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");

                RuleFor(x => x.Address.State)
                    .NotEmpty().WithMessage("State is required")
                    .MaximumLength(100).WithMessage("State cannot exceed 100 characters");
            });
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
} 