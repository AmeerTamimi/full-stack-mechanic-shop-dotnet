using GOATY.Domain.Common.Results;

namespace GOATY.Domain.Identity
{
    public sealed class RefreshTokenErrors
    {
        public static Error IdRequired = Error.Validation(
                                                            code: "RefreshToken.Id.Required",
                                                            description: "Refresh Token Id Is Requried"
                                                        );

        public static Error UserIdRequired = Error.Validation(
                                                            code: "RefreshToken.UserId.Required",
                                                            description: "Refresh Token User Id Is Requried"
                                                        );

        public static Error InvalidToken = Error.Validation(
                                                            code: "RefreshToken.Token.InValid",
                                                            description: "Refresh Token User Id Is Invalid"
                                                        );

        public static Error InvalidDate = Error.Validation(
                                                            code: "RefreshToken.Expiration.InValid",
                                                            description: "Refresh Token Expiration Must Be In The Future"
                                                        );
    }
}
