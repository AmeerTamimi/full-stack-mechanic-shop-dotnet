using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Features.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<AppUserDto>> AuthenticateAsync(string email, string password);
    }
}
