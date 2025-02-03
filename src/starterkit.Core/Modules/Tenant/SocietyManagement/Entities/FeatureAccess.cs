using System;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents feature access control for a society.
    /// Manages which features are enabled/disabled for a specific society.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class FeatureAccess : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the society this feature access belongs to.
        /// Foreign key relationship with Society entity.
        /// </summary>
        public Guid SocietyId { get; set; }

        /// <summary>
        /// Gets or sets the unique key identifying the feature.
        /// Examples: "AMENITY_BOOKING", "VISITOR_MANAGEMENT", "BILLING"
        /// </summary>
        public string FeatureKey { get; set; }

        /// <summary>
        /// Gets or sets whether the feature is enabled.
        /// Controls if the society can use this feature.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date until which this feature is valid.
        /// Used for time-limited feature access.
        /// </summary>
        public DateTime ValidUntil { get; set; }

        /// <summary>
        /// Navigation property for the society this feature access belongs to.
        /// Represents the parent society of this feature access control.
        /// </summary>
        public virtual Society Society { get; set; }
    }
} 