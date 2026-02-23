using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.RefreshTokens;

namespace GOATY.Application.Features.Common.Interfaces
{
    public interface ITokenProvider
    {
        Task<Result<TokenResponse>> GenerateToken(AppUserDto user);
        Task<Result<TokenResponse>> GenerateFromRefreshToken(RefreshToken refreshToken);
    }
}
