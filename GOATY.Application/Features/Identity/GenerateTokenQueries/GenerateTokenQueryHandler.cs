using GOATY.Application.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Identity.GenerateTokenQueries
{
    public sealed class GenerateTokenQueryHandler(
        IIdentityService identityService,
        ITokenProvider tokenProvider) 
        : IRequestHandler<GenerateTokenQuery, Result<TokenResponse>>
    {
        public async Task<Result<TokenResponse>> Handle(GenerateTokenQuery request, CancellationToken cancellationToken)
        {
            var result = await identityService.AuthenticateAsync(request.Email, request.Password);

            if (!result.IsSuccess)
            {
                return result.Errors;
            }

            var token = await tokenProvider.GenerateToken(result.Value);

            if (!token.IsSuccess)
            {
                return token.Errors;
            }
            return token.Value;
        }
    }
}
