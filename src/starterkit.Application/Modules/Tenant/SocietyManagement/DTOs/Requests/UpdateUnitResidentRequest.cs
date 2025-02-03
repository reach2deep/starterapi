namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for updating an existing unit resident record
    /// </summary>
    public class UpdateUnitResidentRequest
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
        /// ID of the resident (User)
        /// </summary>
        public Guid ResidentId { get; set; }

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
    }
} 