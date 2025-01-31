using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Global
{
    public class LoginActivity : BaseEntity
    {
        public Guid UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime LoginTime { get; set; }
        public bool IsSuccessful { get; set; }

        // Navigation property
        public virtual GlobalUser User { get; set; }
    }
} 