using starterkit.starterkit.Core.Enums;

namespace starterkit.starterkit.Application.Modules.Global.TenantManagement.DTOs
{
    public class CreateTenantRequestDto
    {
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string Description { get; set; }
    }

    public class UpdateTenantRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TenantStatus Status { get; set; }
    }

    public class TenantResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string Description { get; set; }
        public TenantStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 