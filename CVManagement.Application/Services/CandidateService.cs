using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CVManagement.Application.Interfaces;
using CVManagement.Application.DTOs.Candidates;

namespace CVManagement.Application.Services
{
    public class CandidateService : ICandidateService
    {
        private readonly ICandidateRepository _candidateRepository;

        public CandidateService(ICandidateRepository candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }
        public async Task<CandidateProfileDto> GetProfileAsync(long userId)
        {
            return await _candidateRepository.GetProfileAsync(userId);
        }


    }
}
