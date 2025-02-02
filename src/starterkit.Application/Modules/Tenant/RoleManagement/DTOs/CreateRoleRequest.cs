namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    public class CreateRoleRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }
} 