using CVManagement.Domain.Entities;

namespace CVManagement.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user, string roleName);
}