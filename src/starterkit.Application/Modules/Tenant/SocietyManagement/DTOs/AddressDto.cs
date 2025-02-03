namespace starterkit.Application.Modules.Tenant.SocietyManagement.DTOs
{
    /// <summary>
    /// DTO for address information
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// Street address including house/building number
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// City or town name
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// State or province name
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Country name
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Postal or ZIP code
        /// </summary>
        public string PostalCode { get; set; }
    }
} 