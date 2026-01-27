namespace MauiMessenger.Core.Entities;

public class ConversationUser
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinedAt { get; set; }

    public Conversation? Conversation { get; set; }
    public User? User { get; set; }
}
