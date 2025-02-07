namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for unit ownership response data
    /// </summary>
    public class UnitOwnershipResponse
    {
        /// <summary>
        /// Unique identifier of the ownership record
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the unit this ownership record belongs to
        /// </summary>
        public Guid UnitId { get; set; }

        /// <summary>
        /// Unit number/name for display purposes
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// ID of the owner (User)
        /// </summary>
        public Guid OwnerId { get; set; }

        /// <summary>
        /// Owner's full name for display purposes
        /// </summary>
        public string OwnerName { get; set; }

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