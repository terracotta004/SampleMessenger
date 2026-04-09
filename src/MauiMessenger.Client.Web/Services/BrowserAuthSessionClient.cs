using MauiMessenger.Client.Shared.Services;
using MauiMessenger.Core.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace MauiMessenger.Client.Web.Services;

public sealed class BrowserAuthSessionClient(IJSRuntime jsRuntime, IConfiguration configuration) : IAuthSessionClient
{
    private readonly string _apiBaseUrl = configuration.GetValue<string>("Api:BaseUrl") ?? "http://localhost:5010";

    public Task<UserDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<UserDto>("messengerAuth.login", cancellationToken, _apiBaseUrl, request).AsTask();

    public Task LogoutAsync(CancellationToken cancellationToken = default)
        => jsRuntime.InvokeVoidAsync("messengerAuth.logout", cancellationToken, _apiBaseUrl).AsTask();

    public Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<UserDto?>("messengerAuth.getCurrentUser", cancellationToken, _apiBaseUrl).AsTask();
}
