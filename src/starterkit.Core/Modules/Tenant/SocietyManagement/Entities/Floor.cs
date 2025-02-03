using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a floor within a block of a society.
    /// Contains multiple residential/commercial units.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class Floor : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the block this floor belongs to.
        /// Foreign key relationship with Block entity.
        /// </summary>
        [Required(ErrorMessage = "Block ID is required")]
        public Guid BlockId { get; set; }

        /// <summary>
        /// Gets or sets the floor number.
        /// Examples: 1, 2, 3 (Ground floor typically 0 or 1)
        /// </summary>
        [Range(0, 200, ErrorMessage = "Floor number must be between 0 and 200")]
        public int FloorNumber { get; set; }

        /// <summary>
        /// Gets or sets the name/identifier of the floor.
        /// Examples: "First Floor", "Penthouse Level"
        /// </summary>
        [Required(ErrorMessage = "Floor name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Floor name must be between 1 and 50 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the total number of units on this floor.
        /// Used for planning and management purposes.
        /// </summary>
        [Range(1, 50, ErrorMessage = "Total units must be between 1 and 50")]
        public int TotalUnits { get; set; }

        /// <summary>
        /// Navigation property for the block this floor belongs to.
        /// Represents the parent block of this floor.
        /// </summary>
        public virtual Block Block { get; set; }

        /// <summary>
        /// Navigation property for units on this floor.
        /// Represents all residential/commercial units on this floor.
        /// </summary>
        public virtual ICollection<Unit> Units { get; set; }

        /// <summary>
        /// Initializes a new instance of the Floor class.
        /// Sets up empty collections for related entities to prevent null reference exceptions.
        /// </summary>
        public Floor()
        {
            Units = new HashSet<Unit>();
        }
    }
} 