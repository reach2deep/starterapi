using starterkit.Core.Entities.Common;

namespace starterkit.Core.Entities.Tenant
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; }

        // Navigation property
        public User User { get; set; }
    }
} 