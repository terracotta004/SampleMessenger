using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;
using MauiMessenger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MauiMessenger.Infrastructure.Repositories;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _dbContext;

    public ConversationRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _dbContext.Conversations.Add(conversation);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return conversation;
    }

    public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Conversations
            .AsNoTracking()
            .Include(c => c.ConversationUsers)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Conversation>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Conversations
            .AsNoTracking()
            .Include(c => c.ConversationUsers)
            .Where(c => c.ConversationUsers.Any(cu => cu.UserId == userId))
            .OrderByDescending(c => c.UpdatedAt)
            .ToListAsync(cancellationToken);
    }
}
