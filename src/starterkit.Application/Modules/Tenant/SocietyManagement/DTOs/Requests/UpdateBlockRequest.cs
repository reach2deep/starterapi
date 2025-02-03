namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for updating an existing block
    /// </summary>
    public class UpdateBlockRequest
    {
        /// <summary>
        /// Unique identifier of the block
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name/identifier of the block (e.g., "A Wing", "Tower 1")
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
    }
} 