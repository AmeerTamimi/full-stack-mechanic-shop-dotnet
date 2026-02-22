using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Jwt;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Queries.GenerateTokenQueries
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

            var token = tokenProvider.GenerateToken(result.Value);
            
            return token.Value;
        }
    }
}
