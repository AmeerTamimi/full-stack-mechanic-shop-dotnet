using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Identity.GenerateByRefreshTokenQueries
{
    public sealed class GenerateByRefreshTokenQueryHandler(
        IAppDbContext context,
        IIdentityService idenetityService,
        ITokenProvider tokenProvider)
        : IRequestHandler<GenerateByRefreshTokenQuery, Result<TokenResponse>>
    {

        public async Task<Result<TokenResponse>> Handle(GenerateByRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var refreshToken = await context.RefreshTokens.SingleOrDefaultAsync(
                                                           r => r.Token == request.Token);

            if(refreshToken is null)
            {
                return Error.NotFound(
                    code: "RefreshToken_NotFound",
                    description: $"The Token {request.Token} was Not Found"
                    );
            }

            var expiration = refreshToken.Expiration;

            if(expiration.Date < DateTimeOffset.Now)
            {
                return RefreshTokenErrors.InvalidDate;
            }

            var appUserResult = await idenetityService.GetByIdAsync(refreshToken.UserId);

            if (!appUserResult.IsSuccess)
            {
                return appUserResult.Errors;
            }

            var tokenResult = await tokenProvider.GenerateToken(appUserResult.Value);

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Errors;
            }

            return tokenResult.Value;
        }
    }
}

