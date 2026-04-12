using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Shared.Services;

public sealed class DefaultAuthSessionClient(IApiClient apiClient) : IAuthSessionClient
{
    public Task<UserDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => apiClient.LoginAsync(request, cancellationToken);

    public Task LogoutAsync(CancellationToken cancellationToken = default)
        => apiClient.LogoutAsync(cancellationToken);

    public Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => apiClient.GetCurrentUserAsync(cancellationToken);
}
