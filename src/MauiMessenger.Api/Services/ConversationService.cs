using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;

namespace MauiMessenger.Api.Services;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;

    public ConversationService(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<ConversationDto> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var participants = request.ParticipantIds
            .Distinct()
            .Select(id => new ConversationUser
            {
                ConversationId = Guid.Empty,
                UserId = id,
                JoinedAt = now
            })
            .ToList();

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            ConversationUsers = participants
        };

        foreach (var participant in participants)
        {
            participant.ConversationId = conversation.Id;
        }

        var saved = await _conversationRepository.AddAsync(conversation, cancellationToken);
        return ToDto(saved);
    }

    public async Task<ConversationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(id, cancellationToken);
        return conversation is null ? null : ToDto(conversation);
    }

    public async Task<IReadOnlyList<ConversationDto>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var conversations = await _conversationRepository.ListByUserIdAsync(userId, cancellationToken);
        return conversations.Select(ToDto).ToList();
    }

    private static ConversationDto ToDto(Conversation conversation)
    {
        var participants = conversation.ConversationUsers.Select(cu => cu.UserId).ToList();
        return new ConversationDto(conversation.Id, conversation.Title, conversation.CreatedAt, conversation.UpdatedAt, participants);
    }
}
