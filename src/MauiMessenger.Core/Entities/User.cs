using Microsoft.AspNetCore.Identity;

namespace MauiMessenger.Core.Entities;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public ParticipantType ParticipantType { get; set; } = ParticipantType.Human;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<ConversationUser> ConversationUsers { get; set; } = new List<ConversationUser>();
}
