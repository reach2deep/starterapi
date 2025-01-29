using starterkit.Core.Entities.Common;
using starterkit.Core.Enums;

namespace starterkit.Core.Entities.Global
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public TenantStatus Status { get; set; }
    }
} 