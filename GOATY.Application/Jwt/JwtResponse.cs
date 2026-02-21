namespace GOATY.Application.Jwt
{
    public sealed class JwtResponse
    {
        public string? AccessToken { get; set; }
        public DateTime Expiry { get; set; }
    }
}
