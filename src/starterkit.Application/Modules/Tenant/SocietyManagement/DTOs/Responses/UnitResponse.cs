namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for unit response data
    /// </summary>
    public class UnitResponse
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
        /// Name of the floor for display purposes in lookup controls
        /// </summary>
        public string FloorName { get; set; }

        /// <summary>
        /// ID of the block this unit belongs to (through floor)
        /// </summary>
        public Guid BlockId { get; set; }

        /// <summary>
        /// Name of the block for display purposes in lookup controls
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// Unit number/identifier
        /// </summary>
        public string UnitNumber { get; set; }

        /// <summary>
        /// Area of the unit in square feet
        /// </summary>
        public int SquareFeet { get; set; }

        /// <summary>
        /// Type of unit
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// When the unit was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of the user who created the unit
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// When the unit was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated the unit
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
} 