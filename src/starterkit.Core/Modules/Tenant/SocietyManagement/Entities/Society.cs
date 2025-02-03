using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant.SocietyManagement.Entities
{
    /// <summary>
    /// Represents a housing society or residential complex in the system.
    /// This is the root entity for society management module.
    /// Inherits from BaseEntity which provides:
    /// - Id (Guid)
    /// - Audit fields (CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
    /// - Soft delete capability (IsActive)
    /// </summary>
    public class Society : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the society.
        /// This is a required field and must be unique within a tenant.
        /// </summary>
        [Required(ErrorMessage = "Society name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Society name must be between 3 and 100 characters")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the official registration number of the society.
        /// This is typically issued by local authorities.
        /// </summary>
        [Required(ErrorMessage = "Registration number is required")]
        [StringLength(50, ErrorMessage = "Registration number cannot exceed 50 characters")]
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Gets or sets the ID of the society's address.
        /// Links to the Address entity for location details.
        /// </summary>
        [Required(ErrorMessage = "Address is required")]
        public Guid AddressId { get; set; }

        /// <summary>
        /// Gets or sets the primary contact email for the society.
        /// Used for official communications.
        /// </summary>
        [Required(ErrorMessage = "Contact email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the primary contact phone number for the society.
        /// Used for official communications.
        /// </summary>
        [Required(ErrorMessage = "Contact phone is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the total number of blocks/buildings in the society.
        /// This helps in planning and management.
        /// </summary>
        [Range(1, 100, ErrorMessage = "Total blocks must be between 1 and 100")]
        public int TotalBlocks { get; set; }

        /// <summary>
        /// Navigation property for the society's address.
        /// Provides full address details through Address entity.
        /// </summary>
        public virtual Address Address { get; set; }

        /// <summary>
        /// Navigation property for blocks in the society.
        /// Represents all blocks/buildings within this society.
        /// </summary>
        public virtual ICollection<Block> Blocks { get; set; }

        /// <summary>
        /// Navigation property for society subscriptions.
        /// Tracks all subscription plans for this society.
        /// </summary>
        public virtual ICollection<SocietySubscription> Subscriptions { get; set; }

        /// <summary>
        /// Navigation property for feature access controls.
        /// Manages which features are enabled for this society.
        /// </summary>
        public virtual ICollection<FeatureAccess> FeatureAccess { get; set; }

        /// <summary>
        /// Initializes a new instance of the Society class.
        /// Sets up empty collections for related entities to prevent null reference exceptions.
        /// </summary>
        public Society()
        {
            Blocks = new HashSet<Block>();
            Subscriptions = new HashSet<SocietySubscription>();
            FeatureAccess = new HashSet<FeatureAccess>();
        }
    }
} 