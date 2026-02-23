namespace GOATY.Application.Features.Common.Configurations
{
    public class JwtConfigurations
    {
        public const string JwtSettings = "JwtSettings"; // name in appsettings.json

        public string? Audience { get; set; }
        public string? Issuer { get; set; }
        public int ExpirationInMinutes { get; set; }
        public string? SecretKey { get; set; }
    }
}
