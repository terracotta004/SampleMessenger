using System.Security.Cryptography;
using System.Text;
using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;

namespace MauiMessenger.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username.Trim(),
            DisplayName = request.DisplayName.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = HashPassword(request.Password),
            CreatedAt = now,
            UpdatedAt = now
        };

        var saved = await _userRepository.AddAsync(user, cancellationToken);
        return ToDto(saved);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : ToDto(user);
    }

    public async Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.ListAsync(cancellationToken);
        return users.Select(ToDto).ToList();
    }

    private static UserDto ToDto(User user)
        => new(user.Id, user.Username, user.DisplayName, user.Email, user.CreatedAt, user.UpdatedAt);

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
