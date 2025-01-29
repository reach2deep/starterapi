using starterkit.Core.Entities.Common;

namespace starterkit.Core.Entities.Tenant
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? AddressId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Address? Address { get; set; }
    }
} 