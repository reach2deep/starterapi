using System;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents ownership information for a unit.
    /// Tracks current and historical ownership details of residential/commercial units.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class UnitOwnership : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the unit this ownership record belongs to.
        /// Foreign key relationship with Unit entity.
        /// </summary>
        [Required(ErrorMessage = "Unit ID is required")]
        public Guid UnitId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the owner (User).
        /// Foreign key relationship with User entity.
        /// </summary>
        [Required(ErrorMessage = "Owner ID is required")]
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Gets or sets the start date of ownership.
        /// When the owner acquired the unit.
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of ownership.
        /// When the owner sold/transferred the unit. Null if current owner.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the status of ownership.
        /// Examples: "Active", "Transferred", "Pending"
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters")]
        public string Status { get; set; }

        /// <summary>
        /// Navigation property for the unit this ownership record belongs to.
        /// Represents the unit being owned.
        /// </summary>
        public virtual Unit Unit { get; set; }

        /// <summary>
        /// Navigation property for the owner.
        /// Represents the user who owns/owned the unit.
        /// </summary>
        public virtual User Owner { get; set; }
    }
} 