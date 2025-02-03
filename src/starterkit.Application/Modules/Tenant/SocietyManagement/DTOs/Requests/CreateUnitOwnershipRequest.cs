namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for creating a new unit ownership record
    /// </summary>
    public class CreateUnitOwnershipRequest
    {
        /// <summary>
        /// ID of the unit this ownership record belongs to
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// ID of the owner (User)
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Start date of the ownership
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the ownership (null for current ownership)
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Status of the ownership (e.g., "Active", "Inactive", "Pending")
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Additional notes or comments about the ownership
        /// </summary>
        public string Notes { get; set; }
    }
} 