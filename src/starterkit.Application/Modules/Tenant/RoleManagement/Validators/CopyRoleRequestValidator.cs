using FluentValidation;
using starterkit.Application.Modules.Tenant.RoleManagement.DTOs;
using starterkit.Core.Modules.Tenant.RoleManagement.Interfaces.Repositories;

namespace starterkit.Application.Modules.Tenant.RoleManagement.Validators
{
    public class CopyRoleRequestValidator : AbstractValidator<CopyRoleRequest>
    {
        private readonly IRoleRepository _roleRepository;

        public CopyRoleRequestValidator(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;

            RuleFor(x => x.NewName)
                .NotEmpty().WithMessage("New role name is required")
                .MaximumLength(100).WithMessage("New role name cannot exceed 100 characters")
                .MustAsync(async (name, cancellation) =>
                {
                    var exists = await _roleRepository.ExistsByNameAsync(name);
                    return !exists;
                }).WithMessage("Role name already exists");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");
        }
    }
} 