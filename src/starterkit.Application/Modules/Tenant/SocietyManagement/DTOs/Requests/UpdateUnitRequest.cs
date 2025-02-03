namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for updating an existing unit
    /// </summary>
    public class UpdateUnitRequest
    {
        /// <summary>
        /// Unique identifier of the unit
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the floor this unit belongs to
        /// </summary>
        public Guid FloorId { get; set; }

        /// <summary>
        /// Unit number/identifier (e.g., "101", "A-1")
        /// </summary>
        public string UnitNumber { get; set; }

        /// <summary>
        /// Area of the unit in square feet
        /// </summary>
        public int SquareFeet { get; set; }

        /// <summary>
        /// Type of unit (e.g., "1BHK", "2BHK", "Commercial")
        /// </summary>
        public string Type { get; set; }
    }
} 