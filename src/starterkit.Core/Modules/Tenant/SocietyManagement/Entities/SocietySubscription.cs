using System;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a subscription plan for a society.
    /// Tracks the subscription details, duration, and payment status.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class SocietySubscription : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the society this subscription belongs to.
        /// Foreign key relationship with Society entity.
        /// </summary>
        public Guid SocietyId { get; set; }

        /// <summary>
        /// Gets or sets the type of subscription plan.
        /// Examples: "Basic", "Premium", "Enterprise"
        /// </summary>
        public string PlanType { get; set; }

        /// <summary>
        /// Gets or sets the start date of the subscription.
        /// When the subscription becomes active.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the subscription.
        /// When the subscription expires.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the subscription amount.
        /// The cost of the subscription plan.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the status of the subscription.
        /// Examples: "Active", "Expired", "Cancelled", "Pending"
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Navigation property for the society this subscription belongs to.
        /// Represents the parent society of this subscription.
        /// </summary>
        public virtual Society Society { get; set; }
    }
} 