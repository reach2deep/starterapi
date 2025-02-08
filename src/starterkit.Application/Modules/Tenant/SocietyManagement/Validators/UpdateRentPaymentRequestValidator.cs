using FluentValidation;
using starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.Validators
{
    public class UpdateRentPaymentRequestValidator : AbstractValidator<UpdateRentPaymentRequest>
    {
        public UpdateRentPaymentRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID is required");

            RuleFor(x => x.Amount)
                .NotEmpty().WithMessage("Amount is required")
                .GreaterThan(0).WithMessage("Amount must be greater than 0");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required");

            RuleFor(x => x.PaidDate)
                .Must((request, paidDate) => !paidDate.HasValue || paidDate.Value <= System.DateTime.UtcNow)
                .WithMessage("Paid date cannot be in the future");

            RuleFor(x => x.PaymentMode)
                .NotEmpty().WithMessage("Payment mode is required")
                .MaximumLength(20).WithMessage("Payment mode cannot exceed 20 characters")
                .Must(mode => new[] { "Cash", "Online", "Check" }.Contains(mode))
                .WithMessage("Payment mode must be Cash, Online, or Check");

            RuleFor(x => x.TransactionReference)
                .MaximumLength(50).WithMessage("Transaction reference cannot exceed 50 characters")
                .When(x => !string.IsNullOrEmpty(x.TransactionReference));

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(20).WithMessage("Status cannot exceed 20 characters")
                .Must(status => new[] { "Pending", "Paid", "Overdue", "Failed" }.Contains(status))
                .WithMessage("Status must be Pending, Paid, Overdue, or Failed");
        }
    }
} 