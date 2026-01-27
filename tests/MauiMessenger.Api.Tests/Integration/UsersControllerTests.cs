using System.Net;
using System.Net.Http.Json;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Api.Tests.Integration;

public class UsersControllerTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersControllerTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateUser_ThenGetById_ReturnsUser()
    {
        var request = new CreateUserRequest("alice", "Alice A", "alice@example.com", "password123");

        var createResponse = await _client.PostAsJsonAsync("/api/users", request);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/users/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var fetched = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal("alice", fetched.Username);
    }

    [Fact]
    public async Task ListUsers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
