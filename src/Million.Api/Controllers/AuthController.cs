using MediatR;
using Microsoft.AspNetCore.Mvc;
using Million.Application.Auth.Commands;
using Million.Application.Auth.DTOs;
using Million.Application.Common.Extensions;

namespace Million.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Login for property owners
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await _mediator.Send(command);

        if (result.IsFailed)
        {
            var statusCode = result.GetStatusCode();
            return StatusCode(statusCode, new
            {
                success = false,
                error = result.GetFirstErrorMessage(),
                statusCode = statusCode
            });
        }

        return Ok(new
        {
            success = true,
            data = result.Value,
            statusCode = 200
        });
    }
}
