using CVManagement.Application.DTOs.Auth;
using CVManagement.Application.Interfaces;
using CVManagement.Application.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CVManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IRegistrationService _registrationService;
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        IRegistrationService registrationService,
        IAuthService authService,
        IOptions<JwtSettings> jwtOptions)
    {
        _registrationService = registrationService;
        _authService = authService;
        _jwtSettings = jwtOptions.Value;
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
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(
                request,
                cancellationToken);

            // Set refresh token in HttpOnly cookie
            SetRefreshTokenCookie(result.RefreshToken);

            // Return only the access token and user info to frontend
            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        try
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token is missing." });
            }

            var result = await _authService.RefreshTokenAsync(
                refreshToken,
                cancellationToken);

            // Set new refresh token in HttpOnly cookie (Token Rotation)
            SetRefreshTokenCookie(result.RefreshToken);

            // Return new access token and user info
            return Ok(result.Response);
        }
        catch (UnauthorizedAccessException ex)
        {
            // Clear the invalid cookie
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                Path = "/api/Auth"
            });

            return Unauthorized(new { message = ex.Message });
        }
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/api/Auth",
                Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                IsEssential = true
            });
    }
}
