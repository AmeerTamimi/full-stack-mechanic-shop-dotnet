using GOATY.Api.Services;
using GOATY.Application.Common.Interfaces;
using MechanicShop.Api.OpenApi.Transformers;
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
            services.AddApiDocumentation();

            return services;
        }

        public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
        {
            string[] versions = ["v1"];

            foreach (var version in versions)
            {
                services.AddOpenApi(version, options =>
                {
                    options.AddDocumentTransformer<VersionInfoTransformer>();

                    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                    options.AddOperationTransformer<BearerSecuritySchemeTransformer>();
                });
            }

            return services;
        }
    }
}