namespace MauiMessenger.Core.DTOs;

public sealed record CreateUserRequest(
    string Username,
    string DisplayName,
    string Email,
    string Password);

public sealed record UserDto(
    Guid Id,
    string Username,
    string DisplayName,
    string Email,
    DateTime CreatedAt,
    DateTime UpdatedAt);
