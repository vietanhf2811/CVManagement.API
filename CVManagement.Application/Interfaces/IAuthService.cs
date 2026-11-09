using CVManagement.Application.DTOs.Auth;

namespace CVManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(
            LoginRequestDto request,
            CancellationToken cancellationToken = default);

        Task<AuthResult> RefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);
    }
}
