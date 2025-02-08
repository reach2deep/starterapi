using System;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a rent payment record for a lease agreement.
    /// Tracks payment details, status, and transaction information.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class RentPayment : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the lease agreement this payment belongs to.
        /// Foreign key relationship with LeaseAgreement entity.
        /// </summary>
        [Required(ErrorMessage = "Lease agreement ID is required")]
        public Guid LeaseAgreementId { get; set; }

        /// <summary>
        /// Gets or sets the payment amount.
        /// </summary>
        [Required(ErrorMessage = "Amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal to 0")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the due date for the payment.
        /// </summary>
        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the actual date when payment was made.
        /// </summary>
        public DateTime? PaidDate { get; set; }

        /// <summary>
        /// Gets or sets the payment mode (e.g., Cash, Online, Check).
        /// </summary>
        [Required(ErrorMessage = "Payment mode is required")]
        [StringLength(20, ErrorMessage = "Payment mode cannot exceed 20 characters")]
        public string PaymentMode { get; set; }

        /// <summary>
        /// Gets or sets the transaction reference number.
        /// </summary>
        [StringLength(50, ErrorMessage = "Transaction reference cannot exceed 50 characters")]
        public string TransactionReference { get; set; }

        /// <summary>
        /// Gets or sets the status of the payment.
        /// Examples: Pending, Paid, Overdue, Failed
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Navigation property for the lease agreement this payment belongs to.
        /// </summary>
        public virtual LeaseAgreement LeaseAgreement { get; set; }
    }
} 