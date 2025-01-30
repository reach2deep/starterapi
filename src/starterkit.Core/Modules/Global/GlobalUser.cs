
using starterkit.Core.Enums;
using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Global
{
    public class GlobalUser : BaseEntity
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MobileNumber { get; set; }
        public string PasswordHash { get; set; }
        public UserType UserType { get; set; }
        public UserStatus Status { get; set; }
    }
}