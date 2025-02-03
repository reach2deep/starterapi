using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class UpdateSocietyRequestValidator : AbstractValidator<UpdateSocietyRequest>
    {
        public UpdateSocietyRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Society ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Society name is required")
                .Length(3, 100).WithMessage("Society name must be between 3 and 100 characters");

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty().WithMessage("Registration number is required")
                .MaximumLength(50).WithMessage("Registration number cannot exceed 50 characters");

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required")
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .EmailAddress().WithMessage("Invalid email address format");

            RuleFor(x => x.ContactPhone)
                .NotEmpty().WithMessage("Contact phone is required")
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .Matches(@"^\+?[\d\s-]+$").WithMessage("Invalid phone number format");

            RuleFor(x => x.TotalBlocks)
                .InclusiveBetween(1, 100).WithMessage("Total blocks must be between 1 and 100");

            RuleFor(x => x.Address)
                .NotNull().WithMessage("Address is required");

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address.StreetAddress)
                    .NotEmpty().WithMessage("Street address is required")
                    .MaximumLength(500).WithMessage("Street address cannot exceed 500 characters");

                RuleFor(x => x.Address.City)
                    .NotEmpty().WithMessage("City is required")
                    .MaximumLength(100).WithMessage("City name cannot exceed 100 characters");

                RuleFor(x => x.Address.State)
                    .MaximumLength(100).WithMessage("State name cannot exceed 100 characters");

                RuleFor(x => x.Address.Country)
                    .NotEmpty().WithMessage("Country is required")
                    .MaximumLength(100).WithMessage("Country name cannot exceed 100 characters");

                RuleFor(x => x.Address.PostalCode)
                    .NotEmpty().WithMessage("Postal code is required")
                    .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters")
                    .Matches(@"^[\d\w\s-]+$").WithMessage("Invalid postal code format");
            });
        }
    }
} 