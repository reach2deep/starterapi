using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? AddressId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Address? Address { get; set; }
    }
}