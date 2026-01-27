using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MauiMessenger.Api.Controllers;

[ApiController]
[Route("api/conversations")]
public class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;

    public ConversationsController(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConversationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var conversation = await _conversationService.GetByIdAsync(id, cancellationToken);
        if (conversation is null)
        {
            return NotFound();
        }

        return Ok(conversation);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ConversationDto>>> ListByUserAsync([FromQuery] Guid? userId, CancellationToken cancellationToken)
    {
        if (userId is null || userId == Guid.Empty)
        {
            return BadRequest("userId query parameter is required.");
        }

        var conversations = await _conversationService.ListByUserIdAsync(userId.Value, cancellationToken);
        return Ok(conversations);
    }

    [HttpPost]
    public async Task<ActionResult<ConversationDto>> CreateAsync([FromBody] CreateConversationRequest request, CancellationToken cancellationToken)
    {
        var created = await _conversationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }
}
