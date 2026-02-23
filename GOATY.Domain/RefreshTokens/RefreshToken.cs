namespace GOATY.Domain.RefreshTokens
{
    public sealed class RefreshToken
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public DateTimeOffset Expiration { get; set; }
    }
}
