using GOATY.Application.Features.Common.Configurations;
using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            var refreshToken = await MakeRefreshToken(user.Email);

            return new TokenResponse
            {
                AccessToken = accessToken,
                Expiry = expires,
                RefreshToken = refreshToken
            };
        }

        public async Task<Result<TokenResponse>> GenerateFromRefreshToken(RefreshToken refreshToken)
        {
            var user = await _identityService.GetByEmailAsync(refreshToken.Email!);

            if (!user.IsSuccess)
            {
                return user.Error;
            }

            return await GenerateToken(user.Value);
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
        private async Task<string> MakeRefreshToken(string email)
        {
            var maskedEmail = UtilityService.MaskEmail(email);
            var r = await _context.RefreshTokens.SingleOrDefaultAsync(r => r.Id == maskedEmail);
            if(r is not null)
            {
                r.Expiration = DateTimeOffset.Now.AddMinutes(300);
            }
            else 
            {
                var refreshToken = new RefreshToken
                {
                    Id = UtilityService.MaskEmail(email),
                    Email = email,
                    Expiration = DateTimeOffset.Now.AddMinutes(300)
                };
                await _context.RefreshTokens.AddAsync(refreshToken);
            }

            await _context.SaveChangesAsync();

            return maskedEmail;
        }
    }
}
