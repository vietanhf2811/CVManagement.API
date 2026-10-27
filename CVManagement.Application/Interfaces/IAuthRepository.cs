using CVManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Application.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserForLoginAsync( string email,CancellationToken cancellationToken = default);
    }
}
