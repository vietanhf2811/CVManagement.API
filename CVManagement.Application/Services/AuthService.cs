using CVManagement.Application.DTOs.Auth;
using CVManagement.Application.Interfaces;
using CVManagement.Application.Settings;
using CVManagement.Domain.Entities;
using Microsoft.Extensions.Options;

namespace CVManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IAuthRepository repository,
        IPasswordHasherService passwordHasher,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IOptions<JwtSettings> jwtOptions)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResult> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _repository.GetUserForLoginAsync(
            email,
            cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var isPasswordValid = _passwordHasher.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        return await CreateAuthResultAsync(user, cancellationToken);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // Hash the incoming token to look up in DB
        var tokenHash = _refreshTokenService.HashToken(refreshToken);

        var storedToken = await _refreshTokenRepository.GetByHashAsync(tokenHash, cancellationToken);

        // Validate: token must exist, not expired, not revoked
        if (storedToken is null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (storedToken.RevokedAt is not null)
        {
            throw new UnauthorizedAccessException("Refresh token has been revoked.");
        }

        if (storedToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedAccessException("Refresh token has expired.");
        }

        // Get user
        var user = await _repository.GetActiveUserByIdAsync(
            (int)storedToken.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found or inactive.");
        }

        // Revoke old refresh token (Token Rotation)
        var newRefreshToken = _refreshTokenService.GenerateToken();
        var newTokenHash = _refreshTokenService.HashToken(newRefreshToken);

        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        storedToken.ReplacedByTokenHash = newTokenHash;

        // Create new refresh token in DB
        var newRefreshTokenEntity = new UserRefreshToken
        {
            UserId = user.Id,
            TokenHash = newTokenHash,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        // Generate new access token
        var roleName = user.Role.Name;
        var accessToken = _jwtTokenService.GenerateToken(user, roleName);

        return new AuthResult
        {
            Response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = roleName,
                AccessToken = accessToken
            },
            RefreshToken = newRefreshToken
        };
    }

    private async Task<AuthResult> CreateAuthResultAsync(User user, CancellationToken cancellationToken)
    {
        var roleName = user.Role.Name;
        var accessToken = _jwtTokenService.GenerateToken(user, roleName);
        var refreshToken = _refreshTokenService.GenerateToken();
        var refreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        var refreshTokenEntity = new UserRefreshToken
        {
            UserId = user.Id,
            TokenHash = _refreshTokenService.HashToken(refreshToken),
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = refreshTokenExpiresAt
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await _refreshTokenRepository.SaveChangesAsync(cancellationToken);

        return new AuthResult
        {
            Response = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Role = roleName,
                AccessToken = accessToken
            },
            RefreshToken = refreshToken
        };
    }
}
