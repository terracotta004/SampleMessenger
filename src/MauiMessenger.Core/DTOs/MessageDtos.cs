namespace MauiMessenger.Core.DTOs;

public sealed record CreateMessageRequest(
    Guid ConversationId,
    Guid SenderId,
    string Content);

public sealed record MessageDto(
    Guid Id,
    Guid ConversationId,
    Guid SenderId,
    string Content,
    DateTime SentAt,
    DateTime? EditedAt,
    bool IsDeleted);
