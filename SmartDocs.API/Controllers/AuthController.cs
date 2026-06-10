using Microsoft.AspNetCore.Mvc;

namespace SmartDocs.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _mediator.Send(
            new SmartDocs.Application.Auth.Commands.LoginCommand(request.Email, request.Password));
        return Ok(result);
    }
}

public record LoginRequest(string Email, string Password);