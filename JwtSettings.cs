namespace TasksAPI;

public class JwtSettings
{
    public string Key { get; set; } = "super_super_secret_key_123456789!@#"; // keep long in real apps
    public string Issuer { get; set; } = "TasksAPI";
    public string Audience { get; set; } = "TasksAPIUser";
    public int ExpiryMinutes { get; set; } = 60;
}
