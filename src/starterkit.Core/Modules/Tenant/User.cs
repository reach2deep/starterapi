
using starterkit.Core.Enums;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant
{
    public class User : BaseEntity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string? MobileNumber { get; set; }
        public UserStatus Status { get; set; }

        // Navigation property
        public UserProfile Profile { get; set; }
    }
}