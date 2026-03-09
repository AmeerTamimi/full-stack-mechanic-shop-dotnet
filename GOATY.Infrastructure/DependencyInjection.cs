using GOATY.Application.Common.Configurations;
using GOATY.Application.Common.Interfaces;
using GOATY.Infrastructure.Data;
using GOATY.Infrastructure.Data.Interceptors;
using GOATY.Infrastructure.Identity;
using GOATY.Infrastructure.Services;
using MechanicShop.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace GOATY.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service , IConfiguration config)
        {
            service.AddDb(config)
                   .AddIdentity()
                   .AddJwtAuthentication(config)
                   .Authorization()
                   .GeneralServices();

            return service;
        }

        public static IServiceCollection AddDb(this IServiceCollection services,
                                               IConfiguration config)
        {
            services.AddScoped<ISaveChangesInterceptor, AuditLoggingInterceptor>();
            services.AddDbContext<AppDbContext>((sp , options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(config.GetSection("ConnectionString").Value);
            });

            services.AddScoped<IAppDbContext, AppDbContext>();
            services.AddScoped<ApplicationDataInitilizer>();

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

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddScoped<ITokenProvider, TokenProvider>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services , IConfiguration config)
        {
            services.AddOptions<JwtConfigurations>()
                    .Bind(config.GetSection(JwtConfigurations.JwtSettings));

            services.ConfigureOptions<JwtBearerConfigurations>();

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

        public static IServiceCollection GeneralServices(this IServiceCollection services)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            services.AddScoped<IWorkOrderRules, WorkOrderRules>();
            services.AddSingleton<IInvoicePdfGenerator, InvoicePdfGenerator>();
            services.AddScoped<INotificationService, NotificationService>();
            return services;
        }
    }
}
