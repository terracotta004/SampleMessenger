using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using MauiMessenger.Client.Web.Services;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Client.Web.Components.Pages;

public partial class Conversations
{
    [Inject] private ApiClient ApiClient { get; set; } = default!;
    [Inject] private CurrentUserState CurrentUserState { get; set; } = default!;
    [Inject] private IConfiguration Configuration { get; set; } = default!;

    private readonly List<UserDto> users = new();
    private readonly List<ConversationDto> conversations = new();
    private readonly List<MessageDto> messages = new();

    private ConversationDto? selectedConversation;
    private Guid? selectedConversationId;
    private bool isLoading;
    private bool isSaving;
    private bool isLoadingMessages;
    private bool isSending;
    private string? errorMessage;
    private NewConversationForm newConversation = new();
    private NewMessageForm newMessage = new();
    private HubConnection? hubConnection;

    private IEnumerable<UserDto> availableUsers
        => users.Where(u => CurrentUserState.CurrentUser is null || u.Id != CurrentUserState.CurrentUser.Id);

    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        if (CurrentUserState.CurrentUser is null)
        {
            return;
        }

        try
        {
            isLoading = true;
            errorMessage = null;

            users.Clear();
            users.AddRange(await ApiClient.GetUsersAsync());

            conversations.Clear();
            var fetched = await ApiClient.GetConversationsByUserAsync(CurrentUserState.CurrentUser.Id);
            conversations.AddRange(fetched.OrderByDescending(c => c.UpdatedAt));
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task CreateConversationAsync()
    {
        if (CurrentUserState.CurrentUser is null || newConversation.UserId is null || newConversation.UserId == Guid.Empty)
        {
            errorMessage = "Please select a user to start a conversation.";
            return;
        }

        try
        {
            isSaving = true;
            errorMessage = null;

            var participantIds = new List<Guid>
            {
                CurrentUserState.CurrentUser.Id,
                newConversation.UserId.Value
            };
            var title = string.IsNullOrWhiteSpace(newConversation.Title)
                ? GetDefaultConversationTitle(newConversation.UserId.Value)
                : newConversation.Title.Trim();

            var request = new CreateConversationRequest(title, participantIds);
            var created = await ApiClient.CreateConversationAsync(request);

            conversations.Insert(0, created);
            newConversation = new NewConversationForm();
            await OpenConversationAsync(created.Id);
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isSaving = false;
        }
    }

    private async Task OpenConversationAsync(Guid conversationId)
    {
        if (selectedConversationId == conversationId)
        {
            return;
        }

        await EnsureHubConnectedAsync();
        if (hubConnection is not null && selectedConversationId is not null)
        {
            await hubConnection.InvokeAsync("LeaveConversation", selectedConversationId.Value);
        }

        selectedConversationId = conversationId;
        selectedConversation = conversations.FirstOrDefault(c => c.Id == conversationId);

        try
        {
            if (hubConnection is not null)
            {
                await hubConnection.InvokeAsync("JoinConversation", conversationId);
            }

            isLoadingMessages = true;
            messages.Clear();
            messages.AddRange(await ApiClient.GetMessagesByConversationAsync(conversationId));
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isLoadingMessages = false;
        }
    }

    private async Task SendMessageAsync()
    {
        if (CurrentUserState.CurrentUser is null || selectedConversationId is null)
        {
            errorMessage = "Please select a conversation before sending a message.";
            return;
        }

        var content = newMessage.Content?.Trim();
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        try
        {
            isSending = true;
            errorMessage = null;

            var request = new CreateMessageRequest(
                selectedConversationId.Value,
                CurrentUserState.CurrentUser.Id,
                content);

            var created = await ApiClient.CreateMessageAsync(request);
            if (!messages.Any(existing => existing.Id == created.Id))
                messages.Add(created);
            newMessage = new NewMessageForm();
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
        finally
        {
            isSending = false;
        }
    }

    private async Task EnsureHubConnectedAsync()
    {
        if (hubConnection is not null)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection.StartAsync();
            }
            return;
        }

        var baseUrl = Configuration.GetValue<string>("Api:BaseUrl") ?? "http://localhost:5010";
        var hubUrl = $"{baseUrl.TrimEnd('/')}/hubs/messages";

        hubConnection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        hubConnection.On<MessageDto>("MessageReceived", message =>
        {
            if (selectedConversationId != message.ConversationId)
            {
                return Task.CompletedTask;
            }

            if (messages.Any(existing => existing.Id == message.Id))
            {
                return Task.CompletedTask;
            }
            if(!messages.Any(existing => existing.Id == message.Id))
                messages.Add(message);
            return InvokeAsync(StateHasChanged);
        });

        hubConnection.Reconnected += async _ =>
        {
            if (selectedConversationId is not null)
            {
                await hubConnection.InvokeAsync("JoinConversation", selectedConversationId.Value);
            }
        };

        await hubConnection.StartAsync();
    }

    private string GetConversationTitle(ConversationDto conversation)
    {
        if (!string.IsNullOrWhiteSpace(conversation.Title))
        {
            return conversation.Title;
        }

        var otherUsers = GetOtherParticipants(conversation).ToList();
        return otherUsers.Count > 0 ? string.Join(", ", otherUsers.Select(u => u.DisplayName)) : "Untitled";
    }

    private string GetConversationParticipants(ConversationDto conversation)
    {
        var otherUsers = GetOtherParticipants(conversation).ToList();
        if (otherUsers.Count == 0)
        {
            return "No other participants";
        }

        return string.Join(", ", otherUsers.Select(u => u.DisplayName));
    }

    private IEnumerable<UserDto> GetOtherParticipants(ConversationDto conversation)
    {
        if (CurrentUserState.CurrentUser is null)
        {
            return Enumerable.Empty<UserDto>();
        }

        var otherIds = conversation.ParticipantIds.Where(id => id != CurrentUserState.CurrentUser.Id);
        return users.Where(u => otherIds.Contains(u.Id));
    }

    private string GetDefaultConversationTitle(Guid otherUserId)
    {
        var user = users.FirstOrDefault(u => u.Id == otherUserId);
        return user is null ? "New conversation" : $"Chat with {user.DisplayName}";
    }

    private string GetUserLabel(Guid userId)
    {
        if (CurrentUserState.CurrentUser is not null && userId == CurrentUserState.CurrentUser.Id)
        {
            return "You";
        }

        var user = users.FirstOrDefault(u => u.Id == userId);
        return user is null ? userId.ToString() : user.DisplayName;
    }

    public async ValueTask DisposeAsync()
    {
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync();
        }
    }

    private sealed class NewConversationForm
    {
        public Guid? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    private sealed class NewMessageForm
    {
        public string Content { get; set; } = string.Empty;
    }
}
