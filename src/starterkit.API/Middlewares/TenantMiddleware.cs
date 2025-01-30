
using starterkit.starterkit.Infrastructure.Services;

namespace starterkit.starterkit.API.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITenantResolver tenantResolver,
            ITenantDatabaseInitializer databaseInitializer)
        {
            // Skip tenant resolution for root-level endpoints
            if (IsRootLevelEndpoint(context))
            {
                await _next(context);
                return;
            }

            // For tenant-specific endpoints, ensure we have authentication
            if (context.Request.Path.StartsWithSegments("/api/v1/tenant") && context.User?.Identity?.IsAuthenticated != true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Authentication required" });
                return;
            }

            var tenant = await tenantResolver.ResolveTenantAsync(context);

            if (tenant == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing tenant identifier. Please ensure tenant ID is provided in the X-Tenant-ID header or JWT token." });
                return;
            }

            if (tenant.Status != Core.Enums.TenantStatus.Active)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "Tenant is not active" });
                return;
            }

            // Initialize tenant database if needed
            await databaseInitializer.InitializeTenantDatabaseAsync(tenant.DatabaseName);

            // Store tenant info in HttpContext items
            context.Items["Tenant"] = tenant.DatabaseName;

            await _next(context);
        }

        private bool IsRootLevelEndpoint(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            return path != null && (
                path.StartsWith("/api/v1/global") ||
                path.StartsWith("/api/auth/login") ||
                path.StartsWith("/health") ||
                path.StartsWith("/swagger")
            );
        }
    }

    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}