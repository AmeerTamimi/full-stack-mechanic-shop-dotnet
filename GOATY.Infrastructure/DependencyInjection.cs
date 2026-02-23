using GOATY.Application.Features.Common.Interfaces;
using GOATY.Infrastructure.Data;
using GOATY.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GOATY.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service , IConfiguration config)
        {
            service.AddDb(config)
                   .AddIdentity()
                   .AddJwtAuthentication()
                   .Authorization();

            return service;
        }

        public static IServiceCollection AddDb(this IServiceCollection services,
                                               IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(config.GetSection("ConnectionString").Value);
            });

            services.AddScoped<IAppDbContext, AppDbContext>();

            return services;
        }
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<AppUser>(options =>
                        {
                            options.Password.RequiredLength = 6;
                            options.Password.RequireDigit = false;
                            options.Password.RequireNonAlphanumeric = false;
                            options.Password.RequireUppercase = false;
                            options.Password.RequireLowercase = false;
                            options.Password.RequiredUniqueChars = 1;
                            options.SignIn.RequireConfirmedAccount = false;
                        })
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(); // configurations will be configured from the configure method on Identity.JwtBearerConfigurations class

            return services;
        }

        public static IServiceCollection Authorization(this IServiceCollection services)
        {
            services.AddAuthorization(); // might add some policies

            return services;
        }


    }
}
