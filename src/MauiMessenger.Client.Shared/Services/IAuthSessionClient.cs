using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Shared.Services;

public interface IAuthSessionClient
{
    Task<UserDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
