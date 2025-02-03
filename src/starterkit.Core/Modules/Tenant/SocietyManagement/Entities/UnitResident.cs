using System;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents resident information for a unit.
    /// Tracks current and historical resident details of residential/commercial units.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class UnitResident : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the unit this resident record belongs to.
        /// Foreign key relationship with Unit entity.
        /// </summary>
        [Required(ErrorMessage = "Unit ID is required")]
        public Guid UnitId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the resident (User).
        /// Foreign key relationship with User entity.
        /// </summary>
        [Required(ErrorMessage = "Resident ID is required")]
        public Guid ResidentId { get; set; }

        /// <summary>
        /// Gets or sets the type of relation with the owner.
        /// Examples: "Self", "Family", "Tenant", "Employee"
        /// </summary>
        [Required(ErrorMessage = "Relation type is required")]
        [StringLength(20, ErrorMessage = "Relation type cannot exceed 20 characters")]
        public string RelationType { get; set; }

        /// <summary>
        /// Gets or sets the start date of residency.
        /// When the resident started living in the unit.
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of residency.
        /// When the resident moved out. Null if current resident.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets whether this is the primary resident.
        /// Each unit should have one primary resident for communications.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Navigation property for the unit this resident record belongs to.
        /// Represents the unit being resided in.
        /// </summary>
        public virtual Unit Unit { get; set; }

        /// <summary>
        /// Navigation property for the resident.
        /// Represents the user who resides/resided in the unit.
        /// </summary>
        public virtual User Resident { get; set; }
    }
} 