using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace starterkit.API.DependencyInjection;

/// <summary>
/// Extensions for configuring Swagger
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger configuration with JWT authentication
    /// </summary>
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Starter Kit API", Version = "v1" });

            // Configure schema IDs to use full type names
            c.CustomSchemaIds(type => type.FullName);

            // Add JWT Authentication
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter JWT Bearer token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            };
            c.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            };
            c.AddSecurityRequirement(securityRequirement);
        });

        // Configure Swagger UI
        services.ConfigureSwaggerGen(options =>
        {
            // Additional Swagger Gen configuration if needed
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger middleware
    /// </summary>
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Starter Kit API V1");
            c.DocExpansion(DocExpansion.None); // Collapse sections by default
            c.DefaultModelsExpandDepth(-1); // Hide schemas section
        });

        return app;
    }
} 