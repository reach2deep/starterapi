using starterkit.Core.Entities.Common;

namespace starterkit.Core.Entities.Tenant
{
    public class Address : BaseEntity
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string? State { get; set; }

        // Navigation property
        public UserProfile UserProfile { get; set; }
    }
} 