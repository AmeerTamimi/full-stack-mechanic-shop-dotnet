using FluentValidation;
using GOATY.Application.Behaviours;
using GOATY.Application.Features.Common.Configurations;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GOATY.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services , IConfiguration config)
        {
            services.AddConfigurations(config)
                    .AddMediator(config);

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions<JwtConfigurations>()
                    .Bind(config.GetSection(JwtConfigurations.JwtSettings));

            return services;
        }

        private static IServiceCollection AddMediator(this IServiceCollection services, IConfiguration config)
        {
            services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(GOATY.Application.AssemblyMarker).Assembly));

            services.AddValidatorsFromAssembly(typeof(GOATY.Application.AssemblyMarker).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            return services;
        }
    }
}
