using MauiMessenger.Core.DTOs;
using MauiMessenger.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MauiMessenger.Api.Controllers;

[ApiController]
[Route("api/messages")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> ListByConversationAsync([FromQuery] Guid? conversationId, CancellationToken cancellationToken)
    {
        if (conversationId is null || conversationId == Guid.Empty)
        {
            return BadRequest("conversationId query parameter is required.");
        }

        var messages = await _messageService.ListByConversationIdAsync(conversationId.Value, cancellationToken);
        return Ok(messages);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateAsync([FromBody] CreateMessageRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _messageService.CreateAsync(request, cancellationToken);
            return Ok(created);
        }
        catch (InvalidOperationException)
        {
            return NotFound("Conversation not found.");
        }
    }
}
