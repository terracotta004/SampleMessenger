using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;

namespace MauiMessenger.Api.Tests.Fakes;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = new();

    public Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.FirstOrDefault(user => user.Id == id));

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => Task.FromResult(_users.FirstOrDefault(user => user.Username == username));

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<User>>(_users.ToList());
}

public sealed class FakeConversationRepository : IConversationRepository
{
    private readonly List<Conversation> _conversations = new();

    public Task<Conversation> AddAsync(Conversation conversation, CancellationToken cancellationToken = default)
    {
        _conversations.Add(conversation);
        return Task.FromResult(conversation);
    }

    public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_conversations.FirstOrDefault(conversation => conversation.Id == id));

    public Task<IReadOnlyList<Conversation>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Conversation>>(
            _conversations
                .Where(conversation => conversation.ConversationUsers.Any(cu => cu.UserId == userId))
                .ToList());
}

public sealed class FakeMessageRepository : IMessageRepository
{
    private readonly List<Message> _messages = new();

    public Task<Message> AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        _messages.Add(message);
        return Task.FromResult(message);
    }

    public Task<Message?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_messages.FirstOrDefault(message => message.Id == id));

    public Task<IReadOnlyList<Message>> ListByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Message>>(
            _messages.Where(message => message.ConversationId == conversationId).ToList());
}
