namespace PRN232.LAB.Services.Models.Options
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = "PRN232.LAB";
        public string Audience { get; set; } = "PRN232.LAB";
        public string Secret { get; set; } = string.Empty;
        public int AccessTokenMinutes { get; set; } = 60;
        public int RefreshTokenDays { get; set; } = 7;
    }
}
