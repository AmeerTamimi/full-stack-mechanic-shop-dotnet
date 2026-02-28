using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;
using GOATY.Domain.Identity;

namespace GOATY.Application.Common.Interfaces
{
    public interface ITokenProvider
    {
        Task<Result<TokenResponse>> GenerateToken(AppUserDto user);
    }
}
