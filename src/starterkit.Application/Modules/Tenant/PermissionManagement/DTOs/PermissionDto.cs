namespace starterkit.Application.Modules.Tenant.PermissionManagement.DTOs
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public bool IsDefault { get; set; }
        public string? Parent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePermissionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public bool IsDefault { get; set; }
        public string? Parent { get; set; }
    }

    public class UpdatePermissionRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public bool IsDefault { get; set; }
        public string? Parent { get; set; }
    }
} 