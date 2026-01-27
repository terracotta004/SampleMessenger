using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;
using MauiMessenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MauiMessenger.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _dbContext;

    public MessageRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return message;
    }

    public Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Message>> ListByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Messages
            .AsNoTracking()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }
}
