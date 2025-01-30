namespace starterkit.Application.DTOs.Global.Auth
{
    public class TenantSelectionRequestDto
    {
        public Guid TenantId { get; set; }
        public string BaseToken { get; set; }
    }

    public class TenantSelectionResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
} 