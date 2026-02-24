using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Identity.GenerateByRefreshTokenQueries
{
    public record class GenerateByRefreshTokenQuery(string Token) : IRequest<Result<TokenResponse>>;
}
