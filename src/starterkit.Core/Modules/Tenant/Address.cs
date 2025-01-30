
using starterkit.starterkit.Core.Modules.Common;

namespace starterkit.starterkit.Core.Modules.Tenant
{
    public class Address : BaseEntity
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string? State { get; set; }
    }
}