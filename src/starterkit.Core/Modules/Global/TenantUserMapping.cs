using starterkit.Core.Entities.Common;

namespace starterkit.Core.Entities.Global
{
    public class TenantUserMapping : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public new bool IsActive { get; set; } = true;

        // Navigation properties
        public Tenant Tenant { get; set; }
        public GlobalUser User { get; set; }
    }
} 