namespace MauiMessenger.Core.Entities;

public class Conversation
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ConversationUser> ConversationUsers { get; set; } = new List<ConversationUser>();
}
