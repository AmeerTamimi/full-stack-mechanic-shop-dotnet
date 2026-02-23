using GOATY.Application.Features.Common.Interfaces;
using GOATY.Application.Features.Identity.DTOs;
using GOATY.Application.Jwt;
using GOATY.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace GOATY.Infrastructure.Identity
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        public IdentityService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<AppUserDto>> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user is null)
            {
                return Error.NotFound(
                    code : "User.Not.Found",
                    description : $"User With Email {UtilityService.MaskEmail(email)} Was Not Found"
                );
            }

            if (!user.EmailConfirmed)
            {
                return Error.Unauthorized(
                    code: "Email.Not.Confirmed",
                    description: $"The Email {UtilityService.MaskEmail(email)} Is Not Confirmed"
                );
            }

            if(! await _userManager.CheckPasswordAsync(user , password))
            {
                return Error.Unauthorized(
                    code: "Invalid.Login.Attempt",
                    description: $"The Email/Password Is Not Correct"
                );
            }

            return new AppUserDto(
                    user.Id,
                    user.Email!,
                    await _userManager.GetRolesAsync(user),
                    await _userManager.GetClaimsAsync(user)
                );  
        }
    }
}
