namespace FIS.Infrastructure.Identity;

public class JwtSettings
{
    public string Issuer { get; set; } = "fis-api";
    public string Audience { get; set; } = "fis-clients";
    public string Key { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
}
