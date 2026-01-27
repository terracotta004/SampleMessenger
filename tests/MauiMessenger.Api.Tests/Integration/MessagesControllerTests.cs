using System.Net;
using System.Net.Http.Json;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Api.Tests.Integration;

public class MessagesControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MessagesControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ListByConversation_RequiresConversationId()
    {
        var response = await _client.GetAsync("/api/messages");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateMessage_ReturnsNotFound_WhenConversationMissing()
    {
        var request = new CreateMessageRequest(Guid.NewGuid(), Guid.NewGuid(), "Hello");

        var response = await _client.PostAsJsonAsync("/api/messages", request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
