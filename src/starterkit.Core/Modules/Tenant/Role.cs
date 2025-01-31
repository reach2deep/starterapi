using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            RolePermissions = new HashSet<RolePermission>();
        }
    }
} 