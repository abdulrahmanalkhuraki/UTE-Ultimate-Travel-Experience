namespace Infrastructure.Security;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string Key { get; set; } = null!;
    public int ExpiresInMinutes { get; set; } = 120;
}
