using CVManagement.Domain.Entities;

namespace CVManagement.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<UserRefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        UserRefreshToken token,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
