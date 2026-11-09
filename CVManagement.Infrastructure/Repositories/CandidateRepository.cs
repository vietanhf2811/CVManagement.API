using CVManagement.Application.DTOs.Candidates;
using CVManagement.Application.Interfaces;
using CVManagement.Domain.Entities;
using CVManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CVManagement.Infrastructure.Repositories
{
    public class CandidateReponsitory : ICandidateRepository
    {
        private readonly CVManagementDbContext _context;
        public async Task<CandidateProfileDto> GetProfileAsync(long userid)
        {
            var candidate = await _context.Candidates
            .Where(c => c.UserId == userid)
            .Select(c => new CandidateProfileDto
            {
                User = new CandidateUserDto
                {
                    FullName = c.User.FullName,
                    Email = c.User.Email,
                    Phone = c.User.Phone
                },

                Skills = c.CandidateSkills
                    .Select(cs => new CandidateSkillDto
                    {
                        SkillId = cs.SkillId,
                        SkillName = cs.Skill.Name
                    }).ToList(),
                Resumes = c.Resumes.Select(r => new CandidateResumeDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    FileUrl = r.OriginalFileName,
                    IsDefault = r.IsDefault,
                    CreatedAt = r.CreatedAt
                }).ToList()

            })
            .FirstOrDefaultAsync();

            if (candidate == null)
            {
                throw new KeyNotFoundException("Candidate profile not found.");
            }
            var profile = new CandidateProfileDto
            {
                User = new CandidateUserDto
                {
                    FullName = candidate.User.FullName,
                    Email = candidate.User.Email,
                    Phone = candidate.User.Phone
                },
                DateOfBirth = candidate.DateOfBirth,
                Address = candidate.Address,
                LinkedInUrl = candidate.LinkedInUrl,
                PortfolioUrl = candidate.PortfolioUrl,
                Summary = candidate.Summary,
                CreatedAt = candidate.CreatedAt,
                UpdatedAt = candidate.UpdatedAt,
                Skills = candidate.Skills
        .Select(cs => new CandidateSkillDto
        {
            SkillId = cs.SkillId,
            SkillName = cs.SkillName
        })
        .ToList(),

                Resumes = candidate.Resumes
        .Select(r => new CandidateResumeDto
        {
            Id = r.Id,
            Title = r.Title,
            FileUrl = r.FileUrl,
            IsDefault = r.IsDefault,
            CreatedAt = r.CreatedAt
        })
        .ToList()
            };
            return profile;
        }

    }
}
