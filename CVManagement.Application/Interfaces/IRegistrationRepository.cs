
using CVManagement.Domain.Entities;

namespace CVManagement.Application.Interfaces;

public interface IRegistrationRepository
{
    Task<bool> EmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByNameAsync(
        string roleName,
        CancellationToken cancellationToken = default);

    Task CreateCandidateAsync(
        User user,
        Candidate candidate,
        CancellationToken cancellationToken = default);
}
