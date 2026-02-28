using GOATY.Application.Common.Configurations;
using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GOATY.Infrastructure.Identity
{
    public sealed class TokenProvider : ITokenProvider
    {
        private readonly IOptions<JwtConfigurations> _jwtConfigurations;
        private readonly IIdentityService _identityService;
        private readonly IAppDbContext _context;
        public TokenProvider(
            IOptions<JwtConfigurations> jwtConfigurations,
            IIdentityService identityService,
            IAppDbContext context)
        {
            _jwtConfigurations = jwtConfigurations;
            _identityService = identityService;
            _context = context;
        }

        public async Task<Result<TokenResponse>> GenerateToken(AppUserDto user)
        {
            var settings = _jwtConfigurations.Value;
            var minutes = settings.TokenExpirationInMinutes;
            var expires = DateTimeOffset.Now.AddMinutes(minutes);

            var accessToken = MakeAccessToken(user);
            var refreshToken = await MakeRefreshToken(user.UserId);

            if (!refreshToken.IsSuccess)
            {
                return refreshToken.Errors;
            }

            return new TokenResponse
            {
                AccessToken = accessToken,
                Expiry = expires,
                RefreshToken = refreshToken.Value
            };
        }
        private async Task<Result<string>> MakeRefreshToken(string userId , CancellationToken ct = default)
        {
            var refreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(
                                                           r => r.UserId == userId);

            var expiration = DateTimeOffset.Now.AddMinutes(300);
            var token = GenerateRefreshToken();
            if (refreshToken is not null)
            {
                refreshToken.Token = token;
                refreshToken.Expiration = expiration;
            }
            else
            {
                
                var tokenId = Guid.NewGuid();
                var refreshTokenResult = RefreshToken.Create(tokenId, userId, token, expiration);

                if (!refreshTokenResult.IsSuccess)
                {
                    return refreshTokenResult.Errors;
                }

                refreshToken = refreshTokenResult.Value;
                await _context.RefreshTokens.AddAsync(refreshToken);
            }

            await _context.SaveChangesAsync(ct);
            return refreshToken.Token!;
        }
        private string MakeAccessToken(AppUserDto user)
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

            return tokenHandler.WriteToken(securityToken);
        }
        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
