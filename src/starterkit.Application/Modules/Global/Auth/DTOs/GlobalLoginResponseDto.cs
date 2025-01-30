namespace starterkit.starterkit.Application.Modules.Global.Auth.DTOs
{
    public class GlobalLoginResponseDto
    {
        public string BaseToken { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IEnumerable<TenantAccessDto> AvailableTenants { get; set; }
    }

    public class TenantAccessDto
    {
        public Guid TenantId { get; set; }
        public string TenantName { get; set; }
        public string Role { get; set; }
    }
}