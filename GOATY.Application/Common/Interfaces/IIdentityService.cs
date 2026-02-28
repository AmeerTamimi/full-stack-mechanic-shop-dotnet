using GOATY.Application.Features.Identity.DTOs;
using GOATY.Domain.Common.Results;

namespace GOATY.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<AppUserDto>> AuthenticateAsync(string email, string password);
        Task<Result<AppUserDto>> GetByIdAsync(string userId);
    }
}
