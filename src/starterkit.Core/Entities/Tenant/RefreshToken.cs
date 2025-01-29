using starterkit.Core.Entities.Common;

namespace starterkit.Core.Entities.Tenant
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public string ReplacedByToken { get; set; }
        public string RevokedReason { get; set; }

        // Navigation property
        public User User { get; set; }
    }
} 