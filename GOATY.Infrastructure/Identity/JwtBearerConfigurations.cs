using GOATY.Application.Common.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GOATY.Infrastructure.Identity
{
    // The Configure Method on this class will be executed from asp.net When we hit an endpoint
    // the method Configures how the "Jwt Bearer" will deal as an authentication scheme with the requests

    // This is the Configuration For Only Bearer !!! And Nothing Else !

    public sealed class JwtBearerConfigurations : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IOptions<JwtConfigurations> _jwtConfigurations;
        public JwtBearerConfigurations(IOptions<JwtConfigurations> jwtSettings)
        {
            _jwtConfigurations = jwtSettings;
        }

        public void Configure(string? name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme) // Which is "Bearer" (So we Only Fk w Bearer)
                return;

            var settings = _jwtConfigurations.Value; // getting the object (JwtSettings)

            options.TokenValidationParameters = new()
            {
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(settings.SecretKey!)
            )
            };
        }
        public void Configure(JwtBearerOptions options)
        {
            Configure("Bearer", options);
        }
    }
}
