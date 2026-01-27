using MauiMessenger.Api.Services;
using Microsoft.AspNetCore.SignalR;

namespace MauiMessenger.Api.Hubs;

public class MessageHub : Hub
{
    public Task JoinConversation(Guid conversationId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, MessageService.GetConversationGroup(conversationId));
    }

    public Task LeaveConversation(Guid conversationId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, MessageService.GetConversationGroup(conversationId));
    }
}
