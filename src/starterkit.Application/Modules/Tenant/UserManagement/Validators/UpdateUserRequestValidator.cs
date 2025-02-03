using FluentValidation;
using starterkit.Application.Modules.Tenant.UserManagement.DTOs;

namespace starterkit.Application.Modules.Tenant.UserManagement.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address.StreetAddress)
                    .NotEmpty().WithMessage("Street address is required")
                    .MaximumLength(100).WithMessage("Street address cannot exceed 100 characters");

                RuleFor(x => x.Address.City)
                    .NotEmpty().WithMessage("City is required")
                    .MaximumLength(50).WithMessage("City cannot exceed 50 characters");

                RuleFor(x => x.Address.State)
                    .NotEmpty().WithMessage("State is required")
                    .MaximumLength(50).WithMessage("State cannot exceed 50 characters");

                RuleFor(x => x.Address.Country)
                    .NotEmpty().WithMessage("Country is required")
                    .MaximumLength(50).WithMessage("Country cannot exceed 50 characters");

                RuleFor(x => x.Address.PostalCode)
                    .NotEmpty().WithMessage("Postal code is required")
                    .MaximumLength(20).WithMessage("Postal code cannot exceed 20 characters");
            });
        }
    }
} 