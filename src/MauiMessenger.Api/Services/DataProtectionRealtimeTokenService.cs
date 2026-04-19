using Microsoft.AspNetCore.DataProtection;

namespace MauiMessenger.Api.Services;

public sealed class DataProtectionRealtimeTokenService : IRealtimeTokenService
{
    private const string Purpose = "MauiMessenger.RealtimeToken.v1";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);
    private readonly IDataProtector _protector;

    public DataProtectionRealtimeTokenService(IDataProtectionProvider dataProtectionProvider)
    {
        _protector = dataProtectionProvider.CreateProtector(Purpose);
    }

    public string CreateToken(Guid userId)
    {
        var expiresAt = DateTimeOffset.UtcNow.Add(TokenLifetime).ToUnixTimeSeconds();
        return _protector.Protect($"{userId:N}|{expiresAt}");
    }

    public bool TryValidateToken(string? token, out Guid userId)
    {
        userId = Guid.Empty;

        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
            var payload = _protector.Unprotect(token);
            var parts = payload.Split('|', 2);
            if (parts.Length != 2)
            {
                return false;
            }

            if (!Guid.TryParseExact(parts[0], "N", out userId))
            {
                return false;
            }

            if (!long.TryParse(parts[1], out var expiresAtUnixSeconds))
            {
                return false;
            }

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expiresAtUnixSeconds);
            if (expiresAt <= DateTimeOffset.UtcNow)
            {
                userId = Guid.Empty;
                return false;
            }

            return true;
        }
        catch
        {
            userId = Guid.Empty;
            return false;
        }
    }
}
