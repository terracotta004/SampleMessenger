using MauiMessenger.Core.Entities;

namespace MauiMessenger.Core.Interfaces;

public interface IConversationRepository
{
    Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Conversation>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
