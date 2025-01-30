using Microsoft.Extensions.DependencyInjection;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.Interfaces;
using starterkit.starterkit.Application.Modules.Global.TenantManagement.Services;
using FluentValidation;
using System.Reflection;

namespace starterkit.starterkit.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register AutoMapper profiles
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Register FluentValidation validators
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register application services
            services.AddScoped<ITenantService, TenantService>();

            return services;
        }
    }
} 