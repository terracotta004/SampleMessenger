namespace MauiMessenger.Core.DTOs;

public sealed record CreateConversationRequest(
    string Title,
    IReadOnlyCollection<Guid> ParticipantIds);

public sealed record ConversationDto(
    Guid Id,
    string Title,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    IReadOnlyCollection<Guid> ParticipantIds);
