using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GOATY.Application.Features.Identity.GenerateFromRefreshTokenQueries
{
    public sealed class GenerateFromRefreshTokenQueryHandler(IAppDbContext context , ITokenProvider tokenProvider) 
        : IRequestHandler<GenerateFromRefreshTokenQuery, Result<TokenResponse>>
    {
        public async Task<Result<TokenResponse>> Handle(GenerateFromRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var refreshToken = await context.RefreshTokens.SingleOrDefaultAsync(
                                                                r => r.Id == request.RefreshToken);

            if(refreshToken is null)
            {
                return Error.NotFound(
                             code: "RefreshToken_NotFound",
                             description: $"RefreshToken {request.RefreshToken} was not Found."
                            );
            }

            var tokenResult = await tokenProvider.GenerateFromRefreshToken(refreshToken);

            if (!tokenResult.IsSuccess)
            {
                return tokenResult.Errors;
            }

            return tokenResult.Value;
        }
    }
}
