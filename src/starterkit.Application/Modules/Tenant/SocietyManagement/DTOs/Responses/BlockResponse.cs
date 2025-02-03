namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for block response data
    /// </summary>
    public class BlockResponse
    {
        /// <summary>
        /// Unique identifier of the block
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the society this block belongs to
        /// </summary>
        public Guid SocietyId { get; set; }

        /// <summary>
        /// Name/identifier of the block
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the block
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Total number of floors in this block
        /// </summary>
        public int TotalFloors { get; set; }

        /// <summary>
        /// When the block was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of the user who created the block
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// When the block was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated the block
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
} 