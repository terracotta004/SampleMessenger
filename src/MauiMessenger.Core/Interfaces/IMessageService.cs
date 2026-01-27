using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Core.Interfaces;

public interface IMessageService
{
    Task<MessageDto> CreateAsync(CreateMessageRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MessageDto>> ListByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
