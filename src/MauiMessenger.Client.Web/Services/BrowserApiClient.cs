using MauiMessenger.Client.Shared.Services;
using MauiMessenger.Core.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

namespace MauiMessenger.Client.Web.Services;

public sealed class BrowserApiClient(IJSRuntime jsRuntime, IConfiguration configuration) : IApiClient
{
    private readonly string _apiBaseUrl = configuration.GetValue<string>("Api:BaseUrl") ?? "http://localhost:5010";

    public Uri BaseAddress => new(_apiBaseUrl, UriKind.Absolute);

    public Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<IReadOnlyList<UserDto>>("messengerApi.getUsers", cancellationToken, _apiBaseUrl).AsTask();

    public Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<UserDto>("messengerApi.createUser", cancellationToken, _apiBaseUrl, request).AsTask();

    public Task<UserDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<UserDto>("messengerAuth.login", cancellationToken, _apiBaseUrl, request).AsTask();

    public Task LogoutAsync(CancellationToken cancellationToken = default)
        => jsRuntime.InvokeVoidAsync("messengerAuth.logout", cancellationToken, _apiBaseUrl).AsTask();

    public Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<UserDto?>("messengerAuth.getCurrentUser", cancellationToken, _apiBaseUrl).AsTask();

    public Task<IReadOnlyList<ConversationDto>> GetConversationsByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<IReadOnlyList<ConversationDto>>("messengerApi.getConversationsByUser", cancellationToken, _apiBaseUrl, userId).AsTask();

    public Task<ConversationDto> CreateConversationAsync(
        CreateConversationRequest request,
        CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<ConversationDto>("messengerApi.createConversation", cancellationToken, _apiBaseUrl, request).AsTask();

    public Task<IReadOnlyList<MessageDto>> GetMessagesByConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<IReadOnlyList<MessageDto>>("messengerApi.getMessagesByConversation", cancellationToken, _apiBaseUrl, conversationId).AsTask();

    public Task<MessageDto> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken cancellationToken = default)
        => jsRuntime.InvokeAsync<MessageDto>("messengerApi.createMessage", cancellationToken, _apiBaseUrl, request).AsTask();
}
