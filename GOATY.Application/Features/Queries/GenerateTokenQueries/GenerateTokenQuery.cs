using GOATY.Application.Jwt;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Queries.GenerateTokenQueries
{
    public record class GenerateTokenQuery(
            string Email,
            string Password
        ) : IRequest<Result<TokenResponse>>;
}
