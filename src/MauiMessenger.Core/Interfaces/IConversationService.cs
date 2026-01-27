using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Core.Interfaces;

public interface IConversationService
{
    Task<ConversationDto> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default);
    Task<ConversationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ConversationDto>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
