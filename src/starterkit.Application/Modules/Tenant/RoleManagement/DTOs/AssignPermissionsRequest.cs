namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    public class AssignPermissionsRequest
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }
} 