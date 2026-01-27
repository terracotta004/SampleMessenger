using MauiMessenger.Api.Hubs;
using MauiMessenger.Api.Services;
using MauiMessenger.Api.Tests.Fakes;
using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Entities;

namespace MauiMessenger.Api.Tests;

public class MessageServiceTests
{
    [Fact]
    public async Task CreateAsync_ThrowsWhenConversationMissing()
    {
        var messageRepository = new FakeMessageRepository();
        var conversationRepository = new FakeConversationRepository();
        var hubContext = new FakeHubContext<MessageHub>();
        var service = new MessageService(messageRepository, conversationRepository, hubContext);
        var request = new CreateMessageRequest(Guid.NewGuid(), Guid.NewGuid(), "Hello");

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_SendsMessageToConversationGroup()
    {
        var messageRepository = new FakeMessageRepository();
        var conversationRepository = new FakeConversationRepository();
        var hubContext = new FakeHubContext<MessageHub>();
        var service = new MessageService(messageRepository, conversationRepository, hubContext);
        var conversationId = Guid.NewGuid();
        await conversationRepository.AddAsync(new Conversation
        {
            Id = conversationId,
            Title = "Chat",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        var request = new CreateMessageRequest(conversationId, Guid.NewGuid(), "  Hi there  ");
        var created = await service.CreateAsync(request);

        var clientProxy = ((FakeHubClients)hubContext.Clients).ClientProxy;
        Assert.Equal($"conversation:{conversationId}", clientProxy.LastGroup);
        Assert.Equal("MessageReceived", clientProxy.LastMethod);
        Assert.NotNull(clientProxy.LastArgs);
        Assert.Equal(created, clientProxy.LastArgs![0]);
        Assert.Equal("Hi there", created.Content);
    }
}
