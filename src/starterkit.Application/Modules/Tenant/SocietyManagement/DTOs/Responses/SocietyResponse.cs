using System;

namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Responses
{
    /// <summary>
    /// DTO for society response data
    /// </summary>
    public class SocietyResponse
    {
        /// <summary>
        /// Unique identifier of the society
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the society
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Official registration number of the society
        /// </summary>
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// Primary contact email for the society
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Primary contact phone number for the society
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Total number of blocks/buildings in the society
        /// </summary>
        public int TotalBlocks { get; set; }

        /// <summary>
        /// Address details of the society
        /// </summary>
        public AddressDto Address { get; set; }

        /// <summary>
        /// When the society was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ID of the user who created the society
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// When the society was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// ID of the user who last updated the society
        /// </summary>
        public Guid? UpdatedBy { get; set; }
    }
} 