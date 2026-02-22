using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.Configurations;
using GOATY.Application.Features.DTOs;
using GOATY.Application.Jwt;
using GOATY.Domain.Common.Results;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GOATY.Infrastructure.Identity
{
    public sealed class TokenProvider : ITokenProvider
    {
        private readonly JwtConfigurations _jwtConfigurations;
        public TokenProvider(JwtConfigurations jwtConfigurations)
        {
            _jwtConfigurations = jwtConfigurations;
        }

        public Result<TokenResponse> GenerateToken(AppUserDto user)
        {
            var expires = DateTime.UtcNow.AddMinutes(_jwtConfigurations.ExpirationInMinutes);

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
                Expires = expires,
                Issuer = _jwtConfigurations.Issuer,
                Audience = _jwtConfigurations.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfigurations.SecretKey!)),
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
