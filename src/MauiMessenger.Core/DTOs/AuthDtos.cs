namespace MauiMessenger.Core.DTOs;

public sealed record LoginRequest(
    string Username,
    string Password);

public sealed record RealtimeTokenResponse(
    string AccessToken);
