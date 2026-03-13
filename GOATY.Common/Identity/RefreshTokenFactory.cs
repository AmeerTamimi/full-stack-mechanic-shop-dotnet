using GOATY.Domain.Common.Results;
using GOATY.Domain.Identity;

namespace GOATY.Tests.Common.Identity
{
    public static class RefreshTokenFactory
    {
        public static Result<RefreshToken> Create(Guid? id = null,
                                                  string? userId = null,
                                                  string? token = null,
                                                  DateTimeOffset? expiration = null)
        {
            return RefreshToken.Create(
                id ?? Guid.NewGuid(),
                userId ?? "UserId",
                token ?? "Access Token",
                expiration ?? DateTimeOffset.Now.AddMinutes(300));
        }
    }
}