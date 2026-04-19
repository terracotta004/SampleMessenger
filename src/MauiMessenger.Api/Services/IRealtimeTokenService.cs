namespace MauiMessenger.Api.Services;

public interface IRealtimeTokenService
{
    string CreateToken(Guid userId);
    bool TryValidateToken(string? token, out Guid userId);
}
