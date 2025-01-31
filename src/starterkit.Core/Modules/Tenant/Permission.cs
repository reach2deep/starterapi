using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public bool IsDefault { get; set; }
    }
} 