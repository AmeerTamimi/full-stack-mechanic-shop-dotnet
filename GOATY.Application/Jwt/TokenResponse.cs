namespace GOATY.Application.Jwt
{
    public sealed class TokenResponse
    {
        public string? AccessToken { get; set; }
        public DateTime Expiry { get; set; }
        public string? RefreshToken { get; set; }
    }
}
