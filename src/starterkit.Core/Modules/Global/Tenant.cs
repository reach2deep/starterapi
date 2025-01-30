
using starterkit.starterkit.Core.Enums;
using starterkit.starterkit.Core.Modules.Common;

namespace starterkit.starterkit.Core.Modules.Global
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public TenantStatus Status { get; set; }
    }
}