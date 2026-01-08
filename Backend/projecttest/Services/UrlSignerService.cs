using System.Security.Cryptography;
using System.Text;

namespace projecttest.Services;

public interface IUrlSignerService
{
    string GenerateSignature(string identifier, long timestamp);
    bool ValidateSignature(string identifier, long timestamp, string signature);
}
public class UrlSignerService : IUrlSignerService
{
    private readonly string _secretKey;
    public UrlSignerService(IConfiguration config)
    {
        _secretKey = config["JwtSettings:Key"] ?? "FallbackSecretKeyDontUseInProd";
    }

    public string GenerateSignature(string identifier, long timestamp)
    {
        var payload = $"{identifier}|{timestamp}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(hash);
    }

    public bool ValidateSignature(string identifier, long timestamp, string signature)
    {
        var creationTIme = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        if (DateTimeOffset.UtcNow > creationTIme.AddMinutes(30))
        {
            return false;
        }

        var expectedSignature = GenerateSignature(identifier, timestamp);
        return signature == expectedSignature;
    }
}
