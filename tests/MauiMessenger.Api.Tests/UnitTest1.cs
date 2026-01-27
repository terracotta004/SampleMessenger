using MauiMessenger.Api.Services;

namespace MauiMessenger.Api.Tests;

public class UnitTest1
{
    [Fact]
    public void ConversationGroupUsesStablePrefix()
    {
        var conversationId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var group = MessageService.GetConversationGroup(conversationId);

        Assert.Equal("conversation:11111111-1111-1111-1111-111111111111", group);
    }
}
