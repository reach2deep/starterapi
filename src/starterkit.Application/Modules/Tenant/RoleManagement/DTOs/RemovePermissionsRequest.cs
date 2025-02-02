namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    public class RemovePermissionsRequest
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }
} 