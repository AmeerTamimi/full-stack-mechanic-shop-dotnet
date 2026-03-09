using GOATY.Api.Services;
using GOATY.Application.Common.Interfaces;
using System.Text.Json.Serialization;

namespace GOATY.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });

            services.AddScoped<IUser, CurrentUser>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
