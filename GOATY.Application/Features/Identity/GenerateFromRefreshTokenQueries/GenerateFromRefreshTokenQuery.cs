using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using MediatR;

namespace GOATY.Application.Features.Identity.GenerateFromRefreshTokenQueries
{
    public record class GenerateFromRefreshTokenQuery(string RefreshToken) 
        : IRequest<Result<TokenResponse>>;
}
