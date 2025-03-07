using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class CreateRentPaymentByUnitRequestValidator : AbstractValidator<CreateRentPaymentByUnitRequest>
    {
        public CreateRentPaymentByUnitRequestValidator()
        {
            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .Must(dueDate => dueDate.Date >= DateTime.UtcNow.Date)
                .WithMessage("Due date cannot be in the past");

            RuleFor(x => x.PaidDate)
                .Must((request, paidDate) => !paidDate.HasValue || paidDate.Value <= DateTime.UtcNow)
                .WithMessage("Paid date cannot be in the future")
                .Must((request, paidDate) => !paidDate.HasValue || paidDate.Value >= request.DueDate)
                .WithMessage("Paid date cannot be before due date");

            RuleFor(x => x.PaymentMode)
                .NotEmpty().WithMessage("Payment mode is required")
                .MaximumLength(20).WithMessage("Payment mode cannot exceed 20 characters")
                .Must(mode => new[] { "Cash", "Online", "Check" }.Contains(mode))
                .WithMessage("Payment mode must be Cash, Online, or Check");

            RuleFor(x => x.TransactionReference)
                .NotEmpty().When(x => x.PaymentMode != "Cash")
                .WithMessage("Transaction reference is required for Online and Check payments")
                .MaximumLength(50).WithMessage("Transaction reference cannot exceed 50 characters")
                .Matches(@"^[a-zA-Z0-9\-_]+$").When(x => !string.IsNullOrEmpty(x.TransactionReference))
                .WithMessage("Transaction reference can only contain letters, numbers, hyphens and underscores");
        }
    }
} 