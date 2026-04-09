using Microsoft.AspNetCore.Components;
using MauiMessenger.Client.Shared.Services;

namespace MauiMessenger.Client.Shared.Components.Layout;

public partial class NavMenu
{
    [Inject] private CurrentUserState CurrentUserState { get; set; } = default!;
    [Inject] private IAuthSessionClient AuthSessionClient { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        CurrentUserState.Changed += OnUserChanged;
        await CurrentUserState.EnsureLoadedAsync(AuthSessionClient);
    }

    private void OnUserChanged()
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task Logout()
    {
        await AuthSessionClient.LogoutAsync();
        CurrentUserState.SetUser(null);
        NavigationManager.NavigateTo("/", forceLoad: true);
    }

    public void Dispose()
    {
        CurrentUserState.Changed -= OnUserChanged;
    }
}
