using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant
{
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        // Navigation properties
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
} 