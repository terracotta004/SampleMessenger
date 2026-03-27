using MauiMessenger.Client.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace MauiMessenger.Client.Shared.Components;

public partial class Routes
{
    [Inject] private CurrentUserState CurrentUserState { get; set; } = default!;
    [Inject] private IAuthSessionClient AuthSessionClient { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        await CurrentUserState.EnsureLoadedAsync(AuthSessionClient);
    }
}
