using FluentValidation;
using GOATY.Application.Common.Behaviours;
using GOATY.Application.Common.Configurations;
using MediatR;
using MediatR.Pipeline;
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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceoptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));

            return services;
        }
    }
}
