using starterkit.Core.Modules.Common;

namespace starterkit.Core.Modules.Tenant
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }
} 