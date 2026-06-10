using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartDocs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DocumentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DocumentStatus? status)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(
            new SmartDocs.Application.Documents.Queries.GetDocumentListQuery(status, userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] UploadDocumentRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _mediator.Send(
            new SmartDocs.Application.Documents.Commands.CreateDocumentCommand(
                request.Title,
                request.File.OpenReadStream(),
                request.File.FileName,
                request.File.ContentType,
                userId));
        return Ok(new { id = result });
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] SubmitRequest request)
    {
        await _mediator.Send(
            new SmartDocs.Application.Documents.Commands.SubmitDocumentCommand(id, request.ReviewerId));
        return Ok();
    }

    [HttpPost("{id}/review")]
    [Authorize(Roles = "Reviewer,Admin")]
    public async Task<IActionResult> Review(Guid id, [FromBody] ReviewRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _mediator.Send(
            new SmartDocs.Application.Documents.Commands.ApproveRejectDocumentCommand(
                id, request.IsApproved, request.Comment, request.Tag, userId));
        return Ok();
    }
}

public record UploadDocumentRequest(string Title, IFormFile File);
public record SubmitRequest(Guid ReviewerId);
public record ReviewRequest(bool IsApproved, string Comment, CommentTag Tag);