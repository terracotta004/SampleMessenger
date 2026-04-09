using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Shared.Services;

public sealed class CurrentUserState
{
    public event Action? Changed;
    public UserDto? CurrentUser { get; private set; }
    public bool IsLoaded { get; private set; }

    public void SetUser(UserDto? user)
    {
        CurrentUser = user;
        IsLoaded = true;
        Changed?.Invoke();
    }

    public async Task EnsureLoadedAsync(IAuthSessionClient authSessionClient, CancellationToken cancellationToken = default)
    {
        if (IsLoaded)
        {
            return;
        }

        var user = await authSessionClient.GetCurrentUserAsync(cancellationToken);
        SetUser(user);
    }
}
