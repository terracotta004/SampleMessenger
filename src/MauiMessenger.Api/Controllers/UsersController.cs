using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MauiMessenger.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> ListAsync(CancellationToken cancellationToken)
    {
        var users = await _userService.ListAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userService.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateAsync([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var created = await _userService.CreateAsync(request, cancellationToken);
        return CreatedAtRoute("GetUserById", new { id = created.Id }, created);
    }
}
