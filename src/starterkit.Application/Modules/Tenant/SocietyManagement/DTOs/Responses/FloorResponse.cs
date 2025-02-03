namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for floor response data
    /// </summary>
    public class FloorResponse
    {
        /// <summary>
        /// Unique identifier of the floor
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the block this floor belongs to
        /// </summary>
        public Guid BlockId { get; set; }

        /// <summary>
        /// Floor number
        /// </summary>
        public int FloorNumber { get; set; }

        /// <summary>
        /// Name/identifier of the floor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Total number of units on this floor
        /// </summary>
        public int TotalUnits { get; set; }

        /// <summary>
        /// When the floor was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of the user who created the floor
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// When the floor was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated the floor
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
} 