using MauiMessenger.Api.Hubs;
using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MauiMessenger.Api.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IHubContext<MessageHub> _hubContext;

    public MessageService(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IHubContext<MessageHub> hubContext)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _hubContext = hubContext;
    }

    public async Task<MessageDto> CreateAsync(CreateMessageRequest request, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
        {
            throw new InvalidOperationException("Conversation not found.");
        }

        var now = DateTime.UtcNow;
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content.Trim(),
            SentAt = now,
            EditedAt = null,
            IsDeleted = false
        };

        var saved = await _messageRepository.AddAsync(message, cancellationToken);
        var dto = ToDto(saved);

        await _hubContext.Clients.Group(GetConversationGroup(request.ConversationId))
            .SendAsync("MessageReceived", dto, cancellationToken);

        return dto;
    }

    public async Task<IReadOnlyList<MessageDto>> ListByConversationIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.ListByConversationIdAsync(conversationId, cancellationToken);
        return messages.Select(ToDto).ToList();
    }

    private static MessageDto ToDto(Message message)
        => new(message.Id, message.ConversationId, message.SenderId, message.Content, message.SentAt, message.EditedAt, message.IsDeleted);

    public static string GetConversationGroup(Guid conversationId)
        => $"conversation:{conversationId}";
}
