using GOATY.Application.Features.Common.Configurations;
using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GOATY.Infrastructure.Identity
{
    public sealed class TokenProvider : ITokenProvider
    {
        private readonly IOptions<JwtConfigurations> _jwtConfigurations;
        public TokenProvider(IOptions<JwtConfigurations> jwtConfigurations)
        {
            _jwtConfigurations = jwtConfigurations;
        }

        public Result<TokenResponse> GenerateToken(AppUserDto user)
        {
            var settings = _jwtConfigurations.Value;

            var minutes = settings.TokenExpirationInMinutes;
            var expires = DateTimeOffset.Now.AddMinutes(minutes);
            var expiresUtc = expires.AddMinutes(minutes).UtcDateTime;

            var claims = new List<Claim>
                {
                new (JwtRegisteredClaimNames.Sub, user.UserId!),
                new (JwtRegisteredClaimNames.Email, user.Email!),
                };

            foreach (var role in user.Roles)
            {
                claims.Add(new(ClaimTypes.Role, role));
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresUtc,
                Issuer = settings.Issuer,
                Audience = settings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey!)),
                    SecurityAlgorithms.HmacSha256Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityToken = tokenHandler.CreateToken(descriptor);

            return new TokenResponse
            {
                AccessToken = tokenHandler.WriteToken(securityToken),
                Expiry = expires,
                RefreshToken = "Hi , My Name Is Refresh token , Nice to meet you guys :)"
            };
        }
    }
}
