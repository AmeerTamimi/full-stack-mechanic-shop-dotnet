using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Identity.GenerateTokenQueries
{
    public record class GenerateTokenQuery(
            string Email,
            string Password
        ) : IRequest<Result<TokenResponse>>;
}
