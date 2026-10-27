using CVManagement.Application.Interfaces;
using CVManagement.Domain.Entities;
using CVManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly CVManagementDbContext _context;

    public AuthRepository(CVManagementDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetUserForLoginAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Users.Include(u => u.Role).AsNoTracking().FirstOrDefaultAsync(u => u.Email == email,cancellationToken);
    }
}