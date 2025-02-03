namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs.Requests
{
    /// <summary>
    /// DTO for creating a new society
    /// </summary>
    public class CreateSocietyRequest
    {
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
    }
} 