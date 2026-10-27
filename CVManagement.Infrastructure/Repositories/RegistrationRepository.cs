
using CVManagement.Application.Interfaces;
using CVManagement.Domain.Entities;
using CVManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Infrastructure.Repositories;

public class RegistrationRepository : IRegistrationRepository
{
    private readonly CVManagementDbContext _context;

    public RegistrationRepository(CVManagementDbContext context)
    {
        _context = context;
    }

    public Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return _context.Users.AnyAsync(
            u => u.Email == email,
            cancellationToken);
    }

    public Task<Role?> GetRoleByNameAsync(
        string roleName,
        CancellationToken cancellationToken = default)
    {
        return _context.Roles.FirstOrDefaultAsync(
            r => r.Name == roleName,
            cancellationToken);
    }

    public async Task CreateCandidateAsync(
        User user,
        Candidate candidate,
        CancellationToken cancellationToken = default)
    {
        user.Candidate = candidate;
        candidate.User = user;

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
