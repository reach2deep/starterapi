using System;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a lease agreement between an owner and a tenant for a unit.
    /// Tracks lease terms, payment details, and status.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class LeaseAgreement : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the unit being leased.
        /// Foreign key relationship with Unit entity.
        /// </summary>
        [Required(ErrorMessage = "Unit ID is required")]
        public Guid UnitId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the owner.
        /// Foreign key relationship with User entity.
        /// </summary>
        [Required(ErrorMessage = "Owner ID is required")]
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the tenant.
        /// Foreign key relationship with User entity.
        /// </summary>
        [Required(ErrorMessage = "Tenant ID is required")]
        public Guid TenantId { get; set; }

        /// <summary>
        /// Gets or sets the start date of the lease.
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the lease.
        /// </summary>
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the monthly rent amount.
        /// </summary>
        [Required(ErrorMessage = "Rent amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Rent amount must be greater than or equal to 0")]
        public decimal RentAmount { get; set; }

        /// <summary>
        /// Gets or sets the security deposit amount.
        /// </summary>
        [Required(ErrorMessage = "Security deposit is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Security deposit must be greater than or equal to 0")]
        public decimal SecurityDeposit { get; set; }

        /// <summary>
        /// Gets or sets the payment frequency (e.g., Monthly, Quarterly, Yearly).
        /// </summary>
        [Required(ErrorMessage = "Payment frequency is required")]
        [StringLength(20, ErrorMessage = "Payment frequency cannot exceed 20 characters")]
        public string PaymentFrequency { get; set; }

        /// <summary>
        /// Gets or sets the notice period in days.
        /// </summary>
        [Required(ErrorMessage = "Notice period is required")]
        [Range(0, 365, ErrorMessage = "Notice period must be between 0 and 365 days")]
        public int NoticePeriodDays { get; set; }

        /// <summary>
        /// Gets or sets the status of the lease agreement.
        /// Examples: Active, Expired, Terminated
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Navigation property for the unit being leased.
        /// </summary>
        public virtual Unit Unit { get; set; }

        /// <summary>
        /// Navigation property for the owner.
        /// </summary>
        public virtual User Owner { get; set; }

        /// <summary>
        /// Navigation property for the tenant.
        /// </summary>
        public virtual User Tenant { get; set; }

        /// <summary>
        /// Navigation property for rent payments associated with this lease.
        /// </summary>
        public virtual ICollection<RentPayment> RentPayments { get; set; }

        public LeaseAgreement()
        {
            RentPayments = new HashSet<RentPayment>();
        }
    }
} 