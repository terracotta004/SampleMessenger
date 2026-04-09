using System.Security.Cryptography;
using System.Text;
using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MauiMessenger.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthController(SignInManager<User> signInManager, UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDto>> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var username = request.Username.Trim();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        var user = await _userManager.Users
            .SingleOrDefaultAsync(candidate => candidate.UserName == username, cancellationToken);
        if (user is null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded && !await TryUpgradeLegacyPasswordAsync(user, request.Password))
        {
            return Unauthorized("Invalid username or password.");
        }

        await EnsureIdentityFieldsAsync(user);
        await _signInManager.SignInAsync(user, isPersistent: false);
        return Ok(ToDto(user));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _signInManager.SignOutAsync();
        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        return Ok(ToDto(user));
    }

    private static UserDto ToDto(User user)
        => new(user.Id, user.UserName ?? string.Empty, user.DisplayName, user.Email ?? string.Empty, user.CreatedAt, user.UpdatedAt);

    private async Task<bool> TryUpgradeLegacyPasswordAsync(User user, string password)
    {
        var legacyHash = HashLegacyPassword(password);
        if (!StringComparer.Ordinal.Equals(user.PasswordHash, legacyHash))
        {
            return false;
        }

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
        await EnsureIdentityFieldsAsync(user);

        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    private async Task EnsureIdentityFieldsAsync(User user)
    {
        var hasChanges = false;

        var normalizedUserName = _userManager.NormalizeName(user.UserName);
        if (!StringComparer.Ordinal.Equals(user.NormalizedUserName, normalizedUserName))
        {
            user.NormalizedUserName = normalizedUserName;
            hasChanges = true;
        }

        var normalizedEmail = _userManager.NormalizeEmail(user.Email);
        if (!StringComparer.Ordinal.Equals(user.NormalizedEmail, normalizedEmail))
        {
            user.NormalizedEmail = normalizedEmail;
            hasChanges = true;
        }

        if (string.IsNullOrWhiteSpace(user.SecurityStamp))
        {
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            hasChanges = true;
        }

        if (string.IsNullOrWhiteSpace(user.ConcurrencyStamp))
        {
            user.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            hasChanges = true;
        }

        if (hasChanges)
        {
            await _userManager.UpdateAsync(user);
        }
    }

    private static string HashLegacyPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
