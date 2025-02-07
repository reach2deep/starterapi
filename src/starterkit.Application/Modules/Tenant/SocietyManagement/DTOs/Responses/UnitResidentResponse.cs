namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for unit resident response data
    /// </summary>
    public class UnitResidentResponse
    {
        /// <summary>
        /// Unique identifier of the resident record
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the unit this resident record belongs to
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// Unit number/name for display purposes in lookup controls
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// ID of the resident (User)
        /// </summary>
        public Guid ResidentId { get; set; }

        /// <summary>
        /// Resident's full name for display purposes in lookup controls
        /// </summary>
        public string ResidentName { get; set; }

        /// <summary>
        /// Start date of residency
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of residency (null for current residents)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Whether this resident is the primary resident of the unit
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Type of residency (e.g., "Owner", "Tenant", "Family Member")
        /// </summary>
        public string ResidencyType { get; set; }

        /// <summary>
        /// Additional notes or comments about the residency
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// When the record was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of the user who created the record
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// When the record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated the record
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
} 