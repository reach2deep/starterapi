using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace starterkit.API.HealthChecks;

public static class HealthCheckResponseWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                data = entry.Value.Data.Select(d => new { key = d.Key, value = d.Value?.ToString() }),
                error = entry.Value.Exception?.Message
            })
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true })
        );
    }
} 