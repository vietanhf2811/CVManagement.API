using CVManagement.Application.Interfaces;
using CVManagement.Domain.Entities;
using CVManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CVManagementDbContext _dbContext;

    public RefreshTokenRepository(CVManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserRefreshToken?> GetByHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task AddAsync(
        UserRefreshToken token,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.UserRefreshTokens.AddAsync(
            token,
            cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
