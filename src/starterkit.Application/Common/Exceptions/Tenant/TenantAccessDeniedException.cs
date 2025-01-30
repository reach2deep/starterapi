namespace starterkit.Application.Common.Exceptions.Tenant
{
    public class TenantAccessDeniedException : Exception
    {
        public Guid TenantId { get; }
        public Guid UserId { get; }

        public TenantAccessDeniedException(Guid tenantId, Guid userId)
            : base($"User {userId} does not have access to tenant {tenantId}")
        {
            TenantId = tenantId;
            UserId = userId;
        }

        public TenantAccessDeniedException(string message) : base(message) { }
        public TenantAccessDeniedException(string message, Exception inner) : base(message, inner) { }
    }
}