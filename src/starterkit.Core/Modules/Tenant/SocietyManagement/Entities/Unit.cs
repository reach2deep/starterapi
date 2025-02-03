using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a residential or commercial unit within a floor.
    /// This is the basic property unit that can be owned or occupied.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class Unit : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the floor this unit belongs to.
        /// Foreign key relationship with Floor entity.
        /// </summary>
        [Required(ErrorMessage = "Floor ID is required")]
        public Guid FloorId { get; set; }

        /// <summary>
        /// Gets or sets the unit number/identifier.
        /// Examples: "101", "A-1", "PH-01"
        /// This should be unique within a society.
        /// </summary>
        [Required(ErrorMessage = "Unit number is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Unit number must be between 1 and 20 characters")]
        public string UnitNumber { get; set; }

        /// <summary>
        /// Gets or sets the area of the unit in square feet.
        /// Used for maintenance calculations and documentation.
        /// </summary>
        [Range(100, 10000, ErrorMessage = "Square feet must be between 100 and 10000")]
        public int SquareFeet { get; set; }

        /// <summary>
        /// Gets or sets the type of unit.
        /// Examples: "1BHK", "2BHK", "Commercial", "Shop"
        /// </summary>
        [Required(ErrorMessage = "Unit type is required")]
        [StringLength(50, ErrorMessage = "Unit type cannot exceed 50 characters")]
        public string Type { get; set; }

        /// <summary>
        /// Navigation property for the floor this unit belongs to.
        /// Represents the parent floor of this unit.
        /// </summary>
        public virtual Floor Floor { get; set; }

        /// <summary>
        /// Navigation property for ownership records of this unit.
        /// Tracks current and historical ownership information.
        /// </summary>
        public virtual ICollection<UnitOwnership> Ownerships { get; set; }

        /// <summary>
        /// Navigation property for resident records of this unit.
        /// Tracks current and historical resident information.
        /// </summary>
        public virtual ICollection<UnitResident> Residents { get; set; }

        /// <summary>
        /// Initializes a new instance of the Unit class.
        /// Sets up empty collections for related entities to prevent null reference exceptions.
        /// </summary>
        public Unit()
        {
            Ownerships = new HashSet<UnitOwnership>();
            Residents = new HashSet<UnitResident>();
        }
    }
} 