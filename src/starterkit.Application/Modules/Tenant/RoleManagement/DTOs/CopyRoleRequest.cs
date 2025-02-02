namespace starterkit.Application.Modules.Tenant.RoleManagement.DTOs
{
    public class CopyRoleRequest
    {
        public string NewName { get; set; }
        public string Description { get; set; }
        public bool CopyPermissions { get; set; } = true;
    }
} 