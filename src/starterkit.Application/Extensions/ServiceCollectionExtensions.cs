using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using starterkit.Application.Services.Global;
using System.Reflection;

namespace starterkit.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Register AutoMapper
            services.AddAutoMapper(assembly);

            // Register FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Register Application Services
            services.Scan(scan => scan
                .FromAssemblyOf<GlobalAuthService>()
                .AddClasses(classes => classes.Where(type => 
                    type.Name.EndsWith("Service") &&
                    !type.IsAbstract &&
                    !type.IsInterface))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
} 