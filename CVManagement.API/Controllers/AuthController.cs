
using CVManagement.Application.DTOs.Auth;
using CVManagement.Application.Interfaces;
using CVManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace CVManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly IAuthService _authService;
    public AuthController(
        IRegistrationService registrationService, IAuthService authService)
    {
        _registrationService = registrationService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _registrationService.RegisterAsync(request, cancellationToken);
            return Created($"/api/users/{result.UserId}", result);
        }
        catch (ArgumentException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Registration Configuration Error", detail: "Unable to register the account at this time.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(
                request,
                cancellationToken);

            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }
    }
}
