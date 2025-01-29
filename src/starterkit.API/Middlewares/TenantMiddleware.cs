using starterkit.Infrastructure.MultiTenancy.Models;
using starterkit.Infrastructure.Services;

namespace starterkit.API.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver)
        {
            // Skip tenant resolution for root-level endpoints
            if (IsRootLevelEndpoint(context))
            {
                await _next(context);
                return;
            }

            var tenant = await tenantResolver.ResolveTenantAsync(context);
            
            if (tenant == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Invalid or missing tenant identifier" });
                return;
            }

            if (tenant.Status != Core.Enums.TenantStatus.Active)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new { error = "Tenant is not active" });
                return;
            }

            // Store tenant info in HttpContext items
            context.Items["Tenant"] = tenant;

            await _next(context);
        }

        private bool IsRootLevelEndpoint(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            return path != null && (
                path.StartsWith("/api/v1/global") ||
                path.StartsWith("/api/auth/login") ||
                path.StartsWith("/health")
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