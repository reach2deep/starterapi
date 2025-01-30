using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using starterkit.Core.Exceptions.Auth;
using starterkit.Core.Exceptions.Tenant;
using starterkit.Core.Models;

namespace starterkit.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception has occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>();

            switch (exception)
            {
                case InvalidCredentialsException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        code: "INVALID_CREDENTIALS"
                    );
                    break;

                case InvalidTokenException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        code: "INVALID_TOKEN"
                    );
                    break;

                case TenantAccessDeniedException tenantEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        code: "TENANT_ACCESS_DENIED",
                        details: new { tenantEx.TenantId, tenantEx.UserId }
                    );
                    break;

                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse<object>.CreateError(
                        message: exception.Message,
                        code: "INVALID_ARGUMENT"
                    );
                    break;

                case ValidationException validationEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse<object>.CreateError(
                        message: "Validation failed",
                        code: "VALIDATION_ERROR",
                        details: validationEx.StackTrace
                    );
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = ApiResponse<object>.CreateError(
                        message: _env.IsDevelopment() ? exception.Message : "An error occurred processing your request.",
                        code: "INTERNAL_SERVER_ERROR",
                        details: _env.IsDevelopment() ? new
                        {
                            exception.StackTrace,
                            InnerException = exception.InnerException?.Message
                        } : null
                    );
                    break;
            }

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }

    // Extension method for easy middleware registration
    public static class GlobalExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        }
    }
} 