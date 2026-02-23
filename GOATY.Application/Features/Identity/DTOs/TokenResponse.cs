namespace GOATY.Application.Features.Identity.DTOs
{
    public sealed class TokenResponse
    {
        public string? AccessToken { get; set; }
        public DateTime Expiry { get; set; }
        public string? RefreshToken { get; set; }
    }
}
