using FluentValidation;
using starterkit.Application.Modules.Tenant.PermissionManagement.DTOs;

namespace starterkit.Application.Modules.Tenant.PermissionManagement.Validators
{
    public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
    {
        public CreatePermissionRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Name is required and must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Description is required and must not exceed 200 characters");

            RuleFor(x => x.Module)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Module is required and must not exceed 50 characters");

            RuleFor(x => x.Action)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Action is required and must not exceed 50 characters");
        }
    }

    public class UpdatePermissionRequestValidator : AbstractValidator<UpdatePermissionRequest>
    {
        public UpdatePermissionRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Name is required and must not exceed 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("Description is required and must not exceed 200 characters");

            RuleFor(x => x.Module)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Module is required and must not exceed 50 characters");

            RuleFor(x => x.Action)
                .NotEmpty()
                .MaximumLength(50)
                .WithMessage("Action is required and must not exceed 50 characters");
        }
    }
} 