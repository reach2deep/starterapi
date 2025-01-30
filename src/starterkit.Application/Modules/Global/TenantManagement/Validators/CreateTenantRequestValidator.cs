using FluentValidation;
using starterkit.Application.Modules.Global.TenantManagement.DTOs;

namespace starterkit.Application.Modules.Global.TenantManagement.Validators
{
    public class CreateTenantRequestValidator : AbstractValidator<CreateTenantRequestDto>
    {
        public CreateTenantRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("^[a-zA-Z0-9-_. ]+$")
                .WithMessage("Name can only contain letters, numbers, spaces, dots, dashes, and underscores");

            RuleFor(x => x.DatabaseName)
                .NotEmpty()
                .MaximumLength(100)
                .Matches("^[a-zA-Z0-9-_]+$")
                .WithMessage("Database name can only contain letters, numbers, dashes, and underscores");

            RuleFor(x => x.ConnectionString)
                .NotEmpty()
                .MaximumLength(500)
                .Must(BeValidConnectionString)
                .WithMessage("Invalid connection string format");

            RuleFor(x => x.Description)
                .MaximumLength(500);
        }

        private bool BeValidConnectionString(string connectionString)
        {
            // Basic validation - checks if it contains basic required parts
            return connectionString.Contains("Server=") || connectionString.Contains("Data Source=")
                && connectionString.Contains("Database=") || connectionString.Contains("Initial Catalog=");
        }
    }
} 