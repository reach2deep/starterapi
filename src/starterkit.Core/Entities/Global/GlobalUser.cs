using starterkit.Core.Entities.Common;
using starterkit.Core.Enums;

namespace starterkit.Core.Entities.Global
{
    public class GlobalUser : BaseEntity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string PasswordHash { get; set; }
        public UserType UserType { get; set; }
        public UserStatus Status { get; set; }
    }
} 