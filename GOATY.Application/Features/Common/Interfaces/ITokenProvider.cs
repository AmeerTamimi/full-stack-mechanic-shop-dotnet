using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Common.Interfaces
{
    public interface ITokenProvider
    {
        Result<TokenResponse> GenerateToken(AppUserDto user);
    }
}
