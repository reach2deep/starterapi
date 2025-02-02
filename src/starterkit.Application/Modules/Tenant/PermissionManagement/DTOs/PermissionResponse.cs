namespace starterkit.Application.Modules.Tenant.PermissionManagement.DTOs
{
    public class PermissionResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
} 