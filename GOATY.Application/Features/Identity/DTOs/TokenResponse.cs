namespace GOATY.Application.Features.Identity.DTOs
{
    public sealed class TokenResponse
    {
        public string? AccessToken { get; set; }
        public DateTimeOffset Expiry { get; set; }
        public string? RefreshToken { get; set; }
    }
}
