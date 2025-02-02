using FluentValidation;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;

namespace starterkit.Application.Modules.Tenant.RoleManagement.Validators
{
    public class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        private readonly IRoleRepository _roleRepository;

        public UpdateRoleRequestValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required")
                .MaximumLength(100).WithMessage("Role name cannot exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
} 