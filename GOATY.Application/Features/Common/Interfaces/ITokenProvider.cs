using GOATY.Application.Features.DTOs;
using GOATY.Application.Jwt;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Common.Interfaces
{
    public interface ITokenProvider
    {
        Result<TokenResponse> GenerateToken(AppUserDto user);
    }
}
