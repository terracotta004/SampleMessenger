using MauiMessenger.Core.DTOs;

namespace MauiMessenger.Core.Interfaces;

public interface IUserService
{
    Task<UserDto?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserDto>> ListAsync(CancellationToken cancellationToken = default);
}
