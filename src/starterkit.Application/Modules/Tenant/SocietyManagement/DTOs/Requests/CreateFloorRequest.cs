namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for creating a new floor
    /// </summary>
    public class CreateFloorRequest
    {
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