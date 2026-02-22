using System.Security.Claims;

namespace GOATY.Application.Features.DTOs
{
    public sealed record AppUserDto(
            string UserId,
            string Email,
            IList<string> Roles,
            IList<Claim> Claims
        );
}
