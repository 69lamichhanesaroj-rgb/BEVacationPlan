namespace Service.Security;

//This is a plain class whose property names match the JSON section, so ASP.NET can fill it in automatically
public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Holiday-BE";
    public string Audience { get; set; } = "Holiday-BE-Client";
    public int ExpiryMinutes { get; set; } = 15;
}