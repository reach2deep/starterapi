namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for updating an existing floor
    /// </summary>
    public class UpdateFloorRequest
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
        /// Floor number (e.g., 1, 2, 3)
        /// </summary>
        public int FloorNumber { get; set; }

        /// <summary>
        /// Name/identifier of the floor (e.g., "First Floor", "Ground Floor")
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Total number of units on this floor
        /// </summary>
        public int TotalUnits { get; set; }
    }
} 