
using CVManagement.Application.DTOs.Auth;

namespace CVManagement.Application.Interfaces;

public interface IRegistrationService
{
    Task<RegisterResponseDto> RegisterAsync(
        RegisterRequestDto request,
        CancellationToken cancellationToken = default);
}
