namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    public class UpdateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
} 