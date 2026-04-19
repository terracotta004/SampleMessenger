using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Shared.Services;

public interface IRealtimeMessageClient : IAsyncDisposable
{
    event Func<MessageDto, Task>? MessageReceived;

    Task ConnectAsync(Uri apiBaseAddress, string accessToken, CancellationToken cancellationToken = default);
    Task JoinConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task LeaveConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
