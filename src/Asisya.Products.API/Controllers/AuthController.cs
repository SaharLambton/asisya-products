using Asisya.Products.Application.DTOs;
using Asisya.Products.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Asisya.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>Authenticate and obtain a JWT token.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        return result.IsSuccess
            ? Ok(result.Data)
            : Unauthorized(new { message = result.ErrorMessage });
    }

    /// <summary>Register a new user.</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAsync(dto, ct);
        if (!result.IsSuccess)
            return result.StatusCode == 409
                ? Conflict(new { message = result.ErrorMessage })
                : BadRequest(new { message = result.ErrorMessage });

        return Created(string.Empty, result.Data);
    }
}
