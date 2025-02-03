using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a block or building within a society.
    /// A block is a physical structure that contains multiple floors.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class Block : BaseEntity
    {
        /// <summary>
        /// Gets or sets the ID of the society this block belongs to.
        /// Foreign key relationship with Society entity.
        /// </summary>
        [Required(ErrorMessage = "Society ID is required")]
        public Guid SocietyId { get; set; }

        /// <summary>
        /// Gets or sets the name/identifier of the block.
        /// Examples: "A Wing", "Tower 1", "Block B"
        /// This should be unique within a society.
        /// </summary>
        [Required(ErrorMessage = "Block name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Block name must be between 1 and 50 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the block.
        /// Optional field to provide additional details about the block.
        /// </summary>
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the total number of floors in this block.
        /// Used for planning and management purposes.
        /// </summary>
        [Range(1, 200, ErrorMessage = "Total floors must be between 1 and 200")]
        public int TotalFloors { get; set; }

        /// <summary>
        /// Navigation property for the society this block belongs to.
        /// Represents the parent society of this block.
        /// </summary>
        public virtual Society Society { get; set; }

        /// <summary>
        /// Navigation property for floors in this block.
        /// Represents all floors contained within this block.
        /// </summary>
        public virtual ICollection<Floor> Floors { get; set; }

        /// <summary>
        /// Initializes a new instance of the Block class.
        /// Sets up empty collections for related entities to prevent null reference exceptions.
        /// </summary>
        public Block()
        {
            Floors = new HashSet<Floor>();
        }
    }
} 