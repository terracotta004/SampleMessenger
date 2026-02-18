using System.Net.Http.Json;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Shared.Services;

public sealed class ApiClient
{
    private readonly HttpClient _httpClient;

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<UserDto>>("api/users", cancellationToken)
            ?? Array.Empty<UserDto>();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/users", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("API returned an empty response.");
    }

    public async Task<UserDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            throw new InvalidOperationException("Invalid username or password.");
        }
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("API returned an empty response.");
    }

    public async Task<IReadOnlyList<ConversationDto>> GetConversationsByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<ConversationDto>>(
                   $"api/conversations?userId={userId}",
                   cancellationToken)
               ?? Array.Empty<ConversationDto>();
    }

    public async Task<ConversationDto> CreateConversationAsync(
        CreateConversationRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/conversations", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ConversationDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("API returned an empty response.");
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessagesByConversationAsync(
        Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<IReadOnlyList<MessageDto>>(
                   $"api/messages?conversationId={conversationId}",
                   cancellationToken)
               ?? Array.Empty<MessageDto>();
    }

    public async Task<MessageDto> CreateMessageAsync(
        CreateMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("api/messages", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MessageDto>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("API returned an empty response.");
    }
}
