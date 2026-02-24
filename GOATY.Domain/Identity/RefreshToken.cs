using GOATY.Domain.Common;
using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Identity
{
    public sealed class RefreshToken : AuditableEntity
    {
        public string UserId { get; set; }
        public string? Token { get; set; }
        public DateTimeOffset Expiration{ get; set; }

        private RefreshToken() { }
        private RefreshToken(Guid id, string userID, string token, DateTimeOffset expiration) 
            : base(id)
        {
            Id = id;
            UserId = userID;
            Token = token;
            Expiration = expiration;
        }

        public static Result<RefreshToken> Create(Guid id , string userId , string token , DateTimeOffset expiration)
        {
            if(Guid.Empty == id)
            {
                return RefreshTokenErrors.IdRequired;
            }
            if (string.IsNullOrEmpty(userId))
            {
                return RefreshTokenErrors.UserIdRequired;
            }
            if (string.IsNullOrEmpty(token))
            {
                return RefreshTokenErrors.InvalidToken;
            }
            if (expiration < DateTimeOffset.Now)
            {
                return RefreshTokenErrors.InvalidDate;
            }

            return new RefreshToken(id, userId, token, expiration);
        }
    }
}
