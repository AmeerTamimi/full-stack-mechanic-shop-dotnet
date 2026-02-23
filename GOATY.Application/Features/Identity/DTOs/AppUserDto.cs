using System.Security.Claims;

namespace GOATY.Application.Features.Identity.DTOs
{
    public sealed record AppUserDto(
            string UserId,
            string Email,
            IList<string> Roles,
            IList<Claim> Claims
        );
}
