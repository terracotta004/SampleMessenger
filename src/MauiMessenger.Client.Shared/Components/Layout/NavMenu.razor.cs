using Microsoft.AspNetCore.Components;
using MauiMessenger.Client.Shared.Services;

namespace MauiMessenger.Client.Shared.Components.Layout;

public partial class NavMenu
{
    [Inject] private CurrentUserState CurrentUserState { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnInitialized()
    {
        CurrentUserState.Changed += OnUserChanged;
    }

    private void OnUserChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private void Logout()
    {
        CurrentUserState.SetUser(null);
        NavigationManager.NavigateTo("/");
    }

    public void Dispose()
    {
        CurrentUserState.Changed -= OnUserChanged;
    }
}
