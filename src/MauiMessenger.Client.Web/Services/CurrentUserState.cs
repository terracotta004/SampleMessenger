using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Web.Services;

public sealed class CurrentUserState
{
    public event Action? Changed;
    public UserDto? CurrentUser { get; private set; }

    public void SetUser(UserDto? user)
    {
        CurrentUser = user;
        Changed?.Invoke();
    }
}
