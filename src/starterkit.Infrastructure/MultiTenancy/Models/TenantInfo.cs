using starterkit.Core.Enums;

namespace starterkit.Infrastructure.MultiTenancy.Models
{
    public class TenantInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public TenantStatus Status { get; set; }
    }
} 