using starterkit.Core.Entities.Common;
using starterkit.Core.Enums;

namespace starterkit.Core.Entities.Tenant
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string MobileNumber { get; set; }
        public UserStatus Status { get; set; }

        // Navigation property
        public UserProfile Profile { get; set; }
    }
} 