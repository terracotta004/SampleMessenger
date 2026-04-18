using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Entities;
using MauiMessenger.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace MauiMessenger.Api.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;

    public UserService(IUserRepository userRepository, UserManager<User> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username.Trim(),
            DisplayName = request.DisplayName.Trim(),
            Email = request.Email.Trim(),
            ParticipantType = request.ParticipantType,
            CreatedAt = now,
            UpdatedAt = now
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new IdentityOperationException(result.Errors.Select(error => error.Description).ToArray());
        }

        var saved = await _userRepository.GetByIdAsync(user.Id, cancellationToken) ?? user;
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
        => new(
            user.Id,
            user.UserName ?? string.Empty,
            user.DisplayName,
            user.Email ?? string.Empty,
            user.ParticipantType,
            user.CreatedAt,
            user.UpdatedAt);
}
