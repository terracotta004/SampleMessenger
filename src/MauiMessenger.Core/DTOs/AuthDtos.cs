namespace MauiMessenger.Core.DTOs;

public sealed record LoginRequest(
    string Username,
    string Password);
