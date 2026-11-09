using CVManagement.Application.DTOs.Candidates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVManagement.Application.Interfaces
{
    public interface ICandidateRepository
    {
       public Task<CandidateProfileDto> GetProfileAsync(long userid);
    }
}
