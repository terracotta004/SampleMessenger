using System.Security.Cryptography;
using System.Text;
using MauiMessenger.Api.Services;
using MauiMessenger.Api.Tests.Fakes;
using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Api.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task CreateAsync_TrimsFieldsAndHashesPassword()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);
        var request = new CreateUserRequest(" alice ", " Alice A ", " alice@example.com ", "password123");

        var created = await service.CreateAsync(request);

        var saved = (await repository.ListAsync()).Single();
        Assert.Equal("alice", saved.Username);
        Assert.Equal("Alice A", saved.DisplayName);
        Assert.Equal("alice@example.com", saved.Email);
        Assert.Equal(created.Id, saved.Id);
        Assert.Equal(saved.CreatedAt, saved.UpdatedAt);
        Assert.Equal(HashPassword("password123"), saved.PasswordHash);
    }

    [Fact]
    public async Task ListAsync_ReturnsDtosForStoredUsers()
    {
        var repository = new FakeUserRepository();
        var service = new UserService(repository);
        var request = new CreateUserRequest("bob", "Bob B", "bob@example.com", "pass");

        await service.CreateAsync(request);

        var users = await service.ListAsync();

        Assert.Single(users);
        Assert.Equal("bob", users[0].Username);
        Assert.Equal("bob@example.com", users[0].Email);
    }

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
